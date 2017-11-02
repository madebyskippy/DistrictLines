using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class districtMap : MonoBehaviour {

    [SerializeField] int MAX_POPULATION_DIFFERENCE;

    //each grid space represented by a cube
    [SerializeField] GameObject cube;

	//the colors that represent the two groups
	[SerializeField] Color[] colors;

	[SerializeField] int numDistricts;

	//starting out with a square map of a set size
	//this is a serializefield so that we can make adjustments as we prototype.
	[SerializeField] int rows;
	[SerializeField] int cols;

	//UI stuff
	[Header("UI stuff")]
	[SerializeField] GameObject UICanvas;
	[SerializeField] GameObject districtIndicatorPrefab;
	[SerializeField] Color[] districtColors;
	[SerializeField] Text districtHeader;
	[SerializeField] Text feedback;
	[SerializeField] Text instructions;
	[SerializeField] Text goalText;
	[SerializeField] Text stats;

	//list of all the game spaces
	List<gridSpace> gridspaces;
    gridSpace[,] gridCoordinates;

	//keeps track of how many people for each group in each district.
	//e.g. districtMakeup[0][0] is how many people are in group A in district 1.
	//	   districtMakeup[1][0] is how many people are in group B in district 1.
	int[][] districtMakeup;
	int[] totalPopulation;

	//for user input and UI
	bool haveScrolled = false;
	bool isSelecting = false; //whether or not the mouse is down and you're selecting
	int currentDistrict = 0; //the district that you're selecting for
	districtIndicator[] indicators;

	//for interfacing with the other 'scenes' like the voting phase
	levelManager LM;

	// Use this for initialization
	void Start () {
		LM = GameObject.FindGameObjectWithTag ("levelManager").GetComponent<levelManager>();
        MAX_POPULATION_DIFFERENCE = 4;
		//start keeping track of the districts.
		districtMakeup = new int[][]{new int[numDistricts], new int[numDistricts]};
		totalPopulation = new int[]{0,0};
		indicators = new districtIndicator[numDistricts];
		for (int i = 0; i < numDistricts; i++) {
			districtMakeup[0] [i] = 0;
			districtMakeup[1] [i] = 0;
			GameObject indicator = Instantiate (districtIndicatorPrefab);
			indicator.transform.position = new Vector3 (50f, 50f+(75f*(numDistricts-1))-75f*i, 0f);
			indicator.transform.SetParent (UICanvas.transform,false);
			indicator.GetComponent<districtIndicator> ().setLabel (districtColors [i], (i+1).ToString());
			indicators [i] = indicator.GetComponent<districtIndicator>();
		}

		float height = indicators [0].GetComponent<RectTransform> ().rect.height;
		height *= 0.75f * UICanvas.GetComponent<Canvas> ().scaleFactor;
		districtHeader.gameObject.transform.position = new Vector3 (indicators[0].transform.position.x,indicators[0].transform.position.y+height, 0f);
        rows = 16;
        cols = 16;
		gridspaces = new List<gridSpace> ();
        gridCoordinates = new gridSpace[rows,cols];
        //setup ();
        Services.LevelLoader.setDistrictMap(this);
        Services.LevelLoader.loadLevel(Level.OHIO_STATE, new Vector2(rows, cols));
        indicators [0].setActive (true);

		goalText.text = "GOAL: "+LM.getInstructions ();

		feedback.text = "";
	}

    public GameObject getCountryPrefab()
    {
        return cube;
    }

    public void addGridSpaceToMap(gridSpace county)
    {
        gridspaces.Add(county);
    }

    public void setCountyPopulation(int[] population)
    {
        totalPopulation[0] += population[0];
        totalPopulation[1] += population[1];
    }

    public void setGridCoordinates(Vector2 coord, gridSpace county)
    {
        if (coord.x < 0 || coord.y < 0) return;
        if (coord.x > rows - 1 || coord.y > cols - 1) return;

        gridCoordinates[(int)coord.x, (int)coord.y] = county;
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
            foreach(gridSpace county in gridspaces)
            {
                if(county.getDistrict() == districtNumber)
                    population += county.getTotalPopulation();
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
            if (north < rows && gridCoordinates[(int)gridSpace.gridPos.x, (int)north] != null)
                northIsContinuous = gridCoordinates[(int)gridSpace.gridPos.x, (int)north].getDistrict() == gridSpace.getDistrict();

            if (south > -1 && gridCoordinates[(int)gridSpace.gridPos.x, (int)south] != null)
                southIsContinuous = gridCoordinates[(int)gridSpace.gridPos.x, (int)south].getDistrict() == gridSpace.getDistrict();

            if (east < cols && gridCoordinates[(int)east, (int)gridSpace.gridPos.y] != null)
                eastIsContinuous = gridCoordinates[(int)east, (int)gridSpace.gridPos.y].getDistrict() == gridSpace.getDistrict();

            if (west > -1 && gridCoordinates[(int)west, (int)gridSpace.gridPos.y] != null)
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
//				Debug.Log (scroll);
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
					feedback.text = "";
                }
                else
                {
					feedback.color = Color.red;
					feedback.text = "Selection is not continuous with current district.";
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

	}

	//finished redistricting
	public void submit(){
		feedback.text = "";
		bool allSpacesAssigned = true;
		for (int i = 0; i < gridspaces.Count; i++)
		{
			if(gridspaces[i] != null &&gridspaces[i].getDistrict() == -1)
			{
				feedback.color = Color.red;
				feedback.text = "Some people have not been assigned a district.";
				Debug.Log("Some people have not been assined a district");
				allSpacesAssigned = false;
				break;
			}
		}

		if(populationDistributionIsValid() && allSpacesAssigned)
		{
			Debug.Log("Population Distribution looks pretty good! Now let's vote!");
			analyze ();
		}
		else
		{
			if (feedback.text != "") {
				feedback.text += "\n";
			}
			feedback.color = Color.red;
			feedback.text += "One district has too many people.";
			Debug.Log("One district has too many people");
		}
	}

	void analyze(){

		LM.setDistrictMakeup (districtMakeup);
		LM.setTotalPopulation (totalPopulation);

		transform.parent.GetComponent<PrototypeSceneScript> ().ChangeScene ();

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
				space.transform.parent = transform;
				int totalInArea = Random.Range(1,10); //total of 9 "people"
				int firstGroup = Random.Range (0, totalInArea);
				if (firstGroup == totalInArea / 2.0f) {
					//prevent ties, we can't handle them right now
					firstGroup++;
				}
				space.GetComponent<gridSpace> ().setGroups(firstGroup,totalInArea-firstGroup);
				space.GetComponent<gridSpace> ().setDistrict (-1);
                space.GetComponent<gridSpace>().setGridPos(i, j);
				gridspaces.Add (space);
                gridCoordinates[i, j] = space.GetComponent<gridSpace>();
				totalPopulation [0] += firstGroup;
				totalPopulation [1] += (totalInArea - firstGroup);
			}
		}
		stats.text = "Circle population is: " + totalPopulation [0];
		stats.text += "\nTriangle population is: " + totalPopulation [1];
	}
}
