using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class districtMap : MonoBehaviour {

    [SerializeField] int MAX_POPULATION_DIFFERENCE;

	//each grid space represented by a cube
	[SerializeField] GameObject cube;

	//the materials that represent the two groups
	[SerializeField] Color[] colors;

	[SerializeField] int numDistricts;

	//starting out with a square map of a set size
	//this is a serializefield so that we can make adjustments as we prototype.
	[SerializeField] int rows;
	[SerializeField] int cols;

	//UI stuff
	[SerializeField] GameObject UICanvas;
	[SerializeField] GameObject districtIndicatorPrefab;
	[SerializeField] Color[] districtColors;

	//list of all the game spaces
	List<gridSpace> gridspaces;
    gridSpace[,] gridCoordinates;

	//keeps track of how many people for each group in each district.
	//e.g. districtMakeup[0][0] is how many people are in group A in district 1.
	//	   districtMakeup[1][0] is how many people are in group B in district 1.
	int[][] districtMakeup;

	//for user input and UI
	bool haveScrolled = false;
	bool isSelecting = false; //whether or not the mouse is down and you're selecting
	int currentDistrict = 0; //the district that you're selecting for
	districtIndicator[] indicators;

	// Use this for initialization
	void Start () {
        MAX_POPULATION_DIFFERENCE = 4;
		//start keeping track of the districts.
		districtMakeup = new int[][]{new int[numDistricts], new int[numDistricts]};
		indicators = new districtIndicator[numDistricts];
		for (int i = 0; i < numDistricts; i++) {
			districtMakeup[0] [i] = 0;
			districtMakeup[1] [i] = 0;
			GameObject indicator = Instantiate (districtIndicatorPrefab);
			indicator.transform.SetParent (UICanvas.transform);
			indicator.transform.localPosition = new Vector3 (0f, 15f+30f*i, 0f);
			indicator.GetComponent<districtIndicator> ().setLabel (districtColors [i]);
			indicators [i] = indicator.GetComponent<districtIndicator>();
		}

		gridspaces = new List<gridSpace> ();
        gridCoordinates = new gridSpace[rows,cols];
		setup ();

		indicators [0].setActive (true);
	}

    public bool isDistrictEmpty(int districtNumber)
    {
        //  Add all the members of each party in
        //  the district we are checking.
        int population = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            population += districtMakeup[i][districtNumber];
        }
        //  If the population is less than 1, the district is empty
        return population < 1;
    }

    public int getDistrictTotalPopulation(int districtNumber)
    {
        // If the district is empty, its population is 0
        if (isDistrictEmpty(districtNumber)) return 0;
        else
        {
            //  Otherwise we add all the members of each party
            //  in the district we are counting to its population
            int population = 0;
            for (int i = 0; i < colors.Length; i++)
            {
                population += districtMakeup[i][districtNumber];
            }
            return population;
        }
    }

    private bool isContinuous(gridSpace gridSpace)
    {
        //  If the district is empty, it is continuous
        if (isDistrictEmpty(currentDistrict)) return true;
        else
        {
            //  Get the N, S, E, W spaces of the map
            float north     = gridSpace.gridPos.y + 1;
            float south     = gridSpace.gridPos.y - 1;
            float east      = gridSpace.gridPos.x + 1;
            float west      = gridSpace.gridPos.x - 1;

            bool northIsContinuous = false;
            bool southIsContinuous = false;
            bool eastIsContinuous = false;
            bool westIsContinuous = false;

            //  Compares the district of the adjacent spaces if withing bounds
            if (north < rows)
                northIsContinuous = gridCoordinates[(int)gridSpace.gridPos.x, (int)north].getDistrict() == gridSpace.getDistrict();

            if (south > -1)
                southIsContinuous = gridCoordinates[(int)gridSpace.gridPos.x, (int)south].getDistrict() == gridSpace.getDistrict();

            if (east < cols)
                eastIsContinuous = gridCoordinates[(int)east, (int)gridSpace.gridPos.y].getDistrict() == gridSpace.getDistrict();

            if (west > -1)
                westIsContinuous = gridCoordinates[(int)west, (int)gridSpace.gridPos.y].getDistrict() == gridSpace.getDistrict();

            //  If at least one space is continuous, we can add the gridspace to the current party
            return northIsContinuous || southIsContinuous || eastIsContinuous || westIsContinuous;
        }
    }

    private bool populationDistributionIsValid()
    {
        int lowestPopulation = int.MaxValue;
        int highestPopulation = int.MinValue;
        for (int i = 0; i < colors.Length; i++)
        {
            int population = getDistrictTotalPopulation(i);
            if(population < lowestPopulation)
            {
                lowestPopulation = population;
            }
            if(highestPopulation < population)
            {
                highestPopulation = population;
            }
        }
        return highestPopulation - lowestPopulation < MAX_POPULATION_DIFFERENCE;
    }


    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		}

        
		//mouse input shenenigans of all sorts
		if (Input.GetMouseButtonDown (0)) {
			//left click
			isSelecting = true;
		}

		if (Input.GetMouseButtonDown (1)) {
			//right click
			nextDistrict(true);
		}

		if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
			if (!haveScrolled) {
				float scroll = Input.GetAxis ("Mouse ScrollWheel");
				Debug.Log (scroll);
				nextDistrict((scroll > 0));
				haveScrolled = true;
			}
		} else {
			haveScrolled = false;
		}

        if (isSelecting && Input.GetMouseButton (0)) {
			//you're holding the mouse down
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
				//you hit a district!
				gridSpace objectHit = hit.transform.gameObject.GetComponent<gridSpace>();
				//check if it was already a district or not
				int prevDistrict = objectHit.getDistrict ();
				if (prevDistrict != -1) {
					//it was previously a district, but now it's not gonna be that anymore
					//so we lower the count
					districtMakeup [objectHit.getGroup ()] [prevDistrict]--;
				}
                //ok now set the district and increase the count
                // check if continuos
                objectHit.setDistrict(currentDistrict);
                if (isContinuous(objectHit))
                {
                    objectHit.setColor(districtColors[currentDistrict]);
                    districtMakeup[objectHit.getGroup()][currentDistrict]++;
                }
                else
                {
                    objectHit.setDistrict(-1);
                }
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			isSelecting = false;

			//this is for UI display
			for (int i = 0; i < numDistricts; i++) {
				indicators [i].setGroups (districtMakeup [0] [i],districtMakeup [1] [i]);
			}

		}

        if(Input.GetKeyDown(KeyCode.Space))
        {
            bool allSpacesAssigned = true;
            for (int i = 0; i < gridspaces.Count; i++)
            {
                if(gridspaces[i].getDistrict() == -1)
                {
                    Debug.Log("Some people have not been assined a district");
                    allSpacesAssigned = false;
                    break;
                }
            }

            if(populationDistributionIsValid() && allSpacesAssigned)
            {
                Debug.Log("Population Distribution looks pretty good! Now let's vote!");
            }
            else
            {
                Debug.Log("One district has too many people");
            }
        }
	}

	void nextDistrict(bool isUp){
		int direction = -1;
		if (isUp)
			direction = 1;
		
		indicators [currentDistrict].setActive (false);
		currentDistrict = (currentDistrict + direction) % numDistricts;		//the % to keep it looping through the districts and not array out of index
		if (currentDistrict < 0) {
			currentDistrict += numDistricts;
		}
		indicators [currentDistrict].setActive (true);
	}

	//sets up the grid of the map
	void setup(){
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
                gridSpace space = Instantiate (cube, new Vector3(i,0,j), Quaternion.identity).GetComponent<gridSpace>();
				int g = Random.Range (0, colors.Length);
				space.GetComponent<gridSpace> ().setGroup (g, colors[g]);
				space.GetComponent<gridSpace> ().setDistrict (-1);
                space.GetComponent<gridSpace>().setGridPos(i, j);
				gridspaces.Add (space);
                gridCoordinates[i, j] = space.GetComponent<gridSpace>();
			}
		}
	}
}
