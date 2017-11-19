using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PoliticalParty { CIRCLE = 0, TRIANGLE }

public class DistrictMap : MonoBehaviour {

    [SerializeField] int MAX_POPULATION_DIFFERENCE;

    //each grid space represented by a cube
    [SerializeField] GameObject countyPrefab;

	//the colors that represent the two groups
	[SerializeField] Color[] colors;

	int numDistricts;

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
	List<County> counties;
    County[,] gridCoordinates;

	//keeps track of how many people for each group in each district.
	//e.g. districtMakeup[(int)PoliticalParty.Circle][0] is how many people are in group A in district 1.
	//	   districtMakeup[(int)PoltiicalParty.Triangle][0] is how many people are in group B in district 1.
	public int[][] districtMakeup;
	int[] totalPopulation;
	//for user input and UI
	bool haveScrolled = false;
	bool isSelecting = false; //whether or not the mouse is down and you're selecting
	int currentDistrict = 0; //the district that you're selecting for
	DistrictIndicator[] indicators;

	//for interfacing with the other 'scenes' like the voting phase
	levelManager LM;

	// Use this for initialization
	public void Init ()
    {
		LM = GameObject.FindGameObjectWithTag ("levelManager").GetComponent<levelManager>();
		numDistricts = LM.getNumDistricts ();
		
        //start keeping track of the districts.
		districtMakeup = new int[][]{new int[numDistricts], new int[numDistricts]};
		totalPopulation = new int[]{0,0};

		indicators = new DistrictIndicator[numDistricts];
		for (int i = 0; i < numDistricts; i++)
        {
			districtMakeup[(int)PoliticalParty.CIRCLE] [i] = 0;
			districtMakeup[(int)PoliticalParty.TRIANGLE] [i] = 0;

            GameObject indicator = Instantiate (districtIndicatorPrefab);

            indicator.transform.position = new Vector3 (50f, 50f + (75f * (numDistricts - 1)) - 75f * i, 0f);
			indicator.transform.SetParent (UICanvas.transform,false);
			indicator.GetComponent<DistrictIndicator>().SetLabel(districtColors[i], (i + 1).ToString());
			indicators[i] = indicator.GetComponent<DistrictIndicator>();
		}

		float height = indicators [0].GetComponent<RectTransform> ().rect.height;
		height *= 0.75f * UICanvas.GetComponent<Canvas> ().scaleFactor;
		districtHeader.gameObject.transform.position = new Vector3 (indicators[0].transform.position.x,indicators[0].transform.position.y+height, 0f);
        rows = (int)TransitionData.Instance.dimensions.x;
        cols = (int)TransitionData.Instance.dimensions.y;

        counties = new List<County> ();
        gridCoordinates = new County[rows, cols];

        indicators [0].SetActive (true);

		goalText.text = "GOAL: "+ LM.getInstructions ();

		feedback.text = "";
    }

    public Vector2[] Directions()
    {
        Vector2[] directions = new Vector2[]
        {
            new Vector2(0,  1),
            new Vector2(1,  0),
            new Vector2(0, -1),
            new Vector2(-1, 0)
        };

        return directions;
    }

    public GameObject GetCountryPrefab()
    {
        return countyPrefab;
    }

    public void AddGridSpaceToMap(County county)
    {
        counties.Add(county);
    }

    public bool CoordIsWithinBounds(Vector2 coord)
    {
        return coord.x >= 0 && coord.x < cols && coord.y >= 0 && coord.y < rows;
    }

    public int CountiesInDirstrict(int district)
    {
        int numOfCounties = 0;
        foreach(County county in counties)
        {
            if (county.getDistrict() == district)
            {
                numOfCounties++;
            }
        }

        return numOfCounties;
    }

    public void setCountyPopulation(int[] population)
    {
        totalPopulation[(int)PoliticalParty.CIRCLE] += population[(int)PoliticalParty.CIRCLE];
		totalPopulation[(int)PoliticalParty.TRIANGLE] += population[(int)PoliticalParty.TRIANGLE];

		stats.text = "Circle population is: " + totalPopulation [(int)PoliticalParty.CIRCLE];
		stats.text += "\nTriangle population is: " + totalPopulation [(int)PoliticalParty.TRIANGLE];

        //  Display this on the screen
        MAX_POPULATION_DIFFERENCE = (int)((totalPopulation[(int)PoliticalParty.CIRCLE] + totalPopulation[(int)PoliticalParty.TRIANGLE]) * 0.05f);
    }

    public void SetGridCoordinates(Vector2 coord, County county)
    {
        if (coord.x < 0 || coord.y < 0) return;
        if (coord.x > rows - 1 || coord.y > cols - 1) return;

        gridCoordinates[(int)coord.x, (int)coord.y] = county;
    }

    public bool IsDistrictEmpty(int districtNumber)
    {
        return GetDistrictTotalPopulation(districtNumber) < 1;
    }

    public int GetDistrictTotalPopulation(int districtNumber)
    {
        //  Otherwise we add all the members of each party
        //  in the district we are counting to its population
        int population = 0;
        foreach (County county in counties)
        {
            if (county.getDistrict() == districtNumber)
                population += county.getTotalPopulation();
        }
        return population;
    }

    public int GetDistrictCirclePopulation(int districtNumber)
    {
        int population = 0;
        foreach (County county in counties)
        {
            if (county.getDistrict() == districtNumber)
                population += county.getCirclePatyPopulation();
        }
        return population;
    }


    public int GetDistrictTrianglePopulation(int districtNumber)
    {
        int population = 0;
        foreach (County county in counties)
        {
            if (county.getDistrict() == districtNumber)
                population += county.getTrianglePartyPopulation();
        }
        return population;
    }

    /*
     *      1: Gather all nodes of a district
     *      2: For the first node check all adjacent spaces
     *      3: Add adjacent spaces with the same county to spacesToBeChecked list
     *      4: If adjacent space has the same district add it to clump
     *      5: Add space to list of checkedSpaces
     *      5: Search the next space in spacesToBeChecked
     *      6: Remove item 
     *      6: End if node space
     */
    

    public bool AllDistrictsContinuityCheck()
    {
        bool[] districtChecks = new bool[numDistricts];
        for(int i = 0; i< districtChecks.Length; i++)
        {
            districtChecks[i] = false;
        }

        County[] firstInsance = new County[numDistricts];
        foreach(County county in counties)
        {
            for (int i = 0; i < firstInsance.Length; i++)
            {
                if (county.getDistrict() == i && firstInsance[i] == null)
                    firstInsance[i] = county;
            }
        }

        for(int i = 0; i < districtChecks.Length;i++)
        {
            districtChecks[i] = ConfirmContinuity(firstInsance[i], new List<County>());
        }

        bool areAllContinuous = true;

        for(int i = 0; i < districtChecks.Length; i++)
        {
            areAllContinuous = areAllContinuous && districtChecks[i];
        }

        return areAllContinuous;
    }

    public bool ConfirmContinuity(County county, List<County> checkedCounties)
    {
        if (county == null) return true;

        checkedCounties.Add(county);

        //  Create list for counties to be checked
        List<County> countiesToBeChecked = new List<County>();

        //  Foreach county in counties to be checked look at its neighbors

        Vector2[] directions = Directions();

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2 adjacentCoord = county.gridPos + directions[i];
            if (CoordIsWithinBounds(adjacentCoord) && gridCoordinates[(int)adjacentCoord.x, (int)adjacentCoord.y] != null &&
                gridCoordinates[(int)adjacentCoord.x, (int)adjacentCoord.y].getDistrict() != -1)
            {
                if (county.getDistrict() == gridCoordinates[(int)adjacentCoord.x, (int)adjacentCoord.y].getDistrict() &&
                    !countiesToBeChecked.Contains(gridCoordinates[(int)adjacentCoord.x, (int)adjacentCoord.y]) && 
                    !checkedCounties.Contains(gridCoordinates[(int)adjacentCoord.x, (int)adjacentCoord.y]))
                {
                    countiesToBeChecked.Add(gridCoordinates[(int)adjacentCoord.x, (int)adjacentCoord.y]);
                }
            }
        }
        

        for (int i = 0; i < countiesToBeChecked.Count; i++)
        {
            ConfirmContinuity(countiesToBeChecked[i], checkedCounties);
        }

        return checkedCounties.Count >= CountiesInDirstrict(county.getDistrict());
    }

    private bool IsContinuous(County gridSpace)
    {
        //  If the district is empty, it is continuous
        //  To stop islanding run a continuity check for all districts before setting one
        
        //  Check if all districts are empty
        //  Check if current district is empty
        //  Check if adjacent spots are continuous
        if (IsDistrictEmpty(currentDistrict)) return true;
        else
        {
            /*
            for (int i  = 0; i < numDistricts; i++)
            {
                if (confirmContinuity(i))
                    return true;
                else
                    return false;
            }
            */

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
                northIsContinuous = gridCoordinates[(int)gridSpace.gridPos.x, (int)north].getDistrict() == currentDistrict;

            if (south > -1 && gridCoordinates[(int)gridSpace.gridPos.x, (int)south] != null)
                southIsContinuous = gridCoordinates[(int)gridSpace.gridPos.x, (int)south].getDistrict() == currentDistrict;

            if (east < cols && gridCoordinates[(int)east, (int)gridSpace.gridPos.y] != null)
                eastIsContinuous = gridCoordinates[(int)east, (int)gridSpace.gridPos.y].getDistrict() == currentDistrict;

            if (west > -1 && gridCoordinates[(int)west, (int)gridSpace.gridPos.y] != null)
                westIsContinuous = gridCoordinates[(int)west, (int)gridSpace.gridPos.y].getDistrict() == currentDistrict;

            //  If at least one space is continuous, we can add the gridspace to the current party
            return northIsContinuous || southIsContinuous || eastIsContinuous || westIsContinuous;
        }
    }

    private bool PopulationDistributionIsValid()
    {
        int lowestPopulation = int.MaxValue;
        int highestPopulation = int.MinValue;
        for (int i = 0; i < colors.Length; i++)
        {
            int population = GetDistrictTotalPopulation(i);
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
    void Update ()
    {
        
		//  Left click
		if (Input.GetMouseButtonDown (0)) isSelecting = true;
        //  Right click
		if (Input.GetMouseButtonDown (1)) NextDistrict(true);

        if( Input.GetKeyDown(KeyCode.Space))
        {
            for(int i = 0; i < numDistricts; i++)
            {
                Debug.Log("District: " + i + " | Population: " + GetDistrictTotalPopulation(i));
            }
        }

		if (Input.GetAxis ("Mouse ScrollWheel") != 0)
        {
			if (!haveScrolled)
            {
				float scroll = Input.GetAxis ("Mouse ScrollWheel");
				NextDistrict((scroll > 0));
				haveScrolled = true;
			}
		}
        else
        {
			haveScrolled = false;
		}

        if (isSelecting && Input.GetMouseButton (0))
        {
			//you're holding the mouse down
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
				//you hit a district!
				County objectHit = hit.transform.gameObject.GetComponent<County>();
				//check if it was already a district or not
				int prevDistrict = objectHit.getDistrict ();
				if (prevDistrict != -1)
                {
					//it was previously a district, but now it's not gonna be that anymore
					//so we lower the count
					districtMakeup [objectHit.getGroup ()] [prevDistrict]--;
				}
                //ok now set the district and increase the count
                // check if continuos
				int objectPreviousDistrict = objectHit.getDistrict();

                Debug.Log("Prev district: " + objectPreviousDistrict);

                if (IsContinuous(objectHit))
                {

                    objectHit.setDistrict(currentDistrict);
                    if (AllDistrictsContinuityCheck())
                    {


                        objectHit.setColor(districtColors[currentDistrict]);
                        districtMakeup[objectHit.getGroup()][currentDistrict]++;
                        UpdatePopulations();
                        feedback.text = "";
                    }
                    else
                    {
                        objectHit.setDistrict(objectPreviousDistrict);
                        feedback.color = Color.red;
                        feedback.text = "Cannot make other districts discontinuous.";
                        UpdatePopulations();
                    }

                }
                else
                {
                    if (objectHit.getDistrict() != currentDistrict)
                    {
                        feedback.color = Color.red;
                        feedback.text = "Selection is not continuous with current district.";
                    }
                }
			}
		}

		if (Input.GetMouseButtonUp (0))
        {
			isSelecting = false;

            //UpdatePopulations();
        }
	}

    public void UpdatePopulations()
    {
        //this is for UI display
        for (int i = 0; i < numDistricts; i++)
        {
            int circlePopulation = GetDistrictCirclePopulation(i);
            int trianglePopulation = GetDistrictTrianglePopulation(i);
            indicators[i].SetPopulationBar(circlePopulation, trianglePopulation);
            int population = GetDistrictTotalPopulation(i);
            indicators[i].SetTotalPopulationText(population);
        }
    }

	//finished redistricting
	public void Submit()
    {
		feedback.text = "";
		bool allSpacesAssigned = true;
		for (int i = 0; i < counties.Count; i++)
		{
			if (counties [i] != null) {
				if (counties [i].getDistrict () == -1)
                {
					feedback.color = Color.red;
					feedback.text = "Some people have not been assigned a district.";

                    Debug.Log ("Some people have not been assined a district");
					Debug.Log (counties [i].gridPos.x + ", " + counties [i].gridPos.y);

                    allSpacesAssigned = false;
					break;
				}
			}     
		}

		if(allSpacesAssigned)
		{
			if (PopulationDistributionIsValid ())
            {
				Debug.Log("Population Distribution looks pretty good! Now let's vote!");
				Analyze ();
			}
            else
            {
				if (feedback.text != "")
                {
					feedback.text += "\n";
				}
				feedback.color = Color.red;
				feedback.text += "One district has too many people.";
				Debug.Log("One district has too many people");
			}
		}
	}

	void Analyze()
    {
		LM.setDistrictMakeup (districtMakeup);
		LM.setTotalPopulation (totalPopulation);

		transform.parent.GetComponent<PrototypeSceneScript> ().ChangeScene ();
	}

	void NextDistrict(bool isUp)
    {
		int direction = -1;

        if (isUp)   direction = 1;
		
		indicators [currentDistrict].SetActive(false);
		currentDistrict = (currentDistrict + direction) % numDistricts;     //the % to keep it looping through the districts and not array out of index

        if (currentDistrict < 0)
        {
			currentDistrict += numDistricts;
		}
		indicators [currentDistrict].SetActive(true);
	}

	//sets up the grid of the map
	void Setup()
    {
		for (int i = 0; i < rows; i++)
        {
			for (int j = 0; j < cols; j++)
            {
                County newCounty = Instantiate (countyPrefab, new Vector3(i,0,j), Quaternion.identity).GetComponent<County>();

                newCounty.transform.parent = transform;

                int totalCountyPopulation = Random.Range(1,10); //total of 9 "people"

                int newCountyCirclePopulation = Random.Range (0, totalCountyPopulation);
				if (newCountyCirclePopulation == totalCountyPopulation / 2.0f)
                {
					//prevent ties, we can't handle them right now
					newCountyCirclePopulation++;
				}

                newCounty.setGroups(newCountyCirclePopulation,totalCountyPopulation - newCountyCirclePopulation);
				newCounty.setDistrict (-1);
                newCounty.setGridPos(i, j);

                newCounty.setCirclePartyPopulation(newCountyCirclePopulation);
                newCounty.setTrianglePartyPopulation((totalCountyPopulation - newCountyCirclePopulation));

                counties.Add (newCounty);

                gridCoordinates[i, j] = newCounty.GetComponent<County>();

                totalPopulation [(int)PoliticalParty.CIRCLE] += newCountyCirclePopulation;
				totalPopulation [(int)PoliticalParty.TRIANGLE] += (totalCountyPopulation - newCountyCirclePopulation);
			}
		}
		stats.text = "Circle population is: " + totalPopulation [(int)PoliticalParty.CIRCLE];
		stats.text += "\nTriangle population is: " + totalPopulation [(int)PoliticalParty.TRIANGLE];
	}
}
