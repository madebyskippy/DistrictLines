using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PoliticalParty { CIRCLE = 0, TRIANGLE }

public class DistrictMap : MonoBehaviour {

    [SerializeField] float MAX_POPULATION_DIFFERENCE;

    //each grid space represented by a cube
    [SerializeField] GameObject countyPrefab;

	//the colors that represent the two groups
	[SerializeField] Color[] colors;

	int numDistricts;
    int mostPopulated;
    int leastPopulated;

	//starting out with a square map of a set size
	//this is a serializefield so that we can make adjustments as we prototype.
	[SerializeField] int rows;
	[SerializeField] int cols;

	//UI stuff
	[Header("UI stuff")]
	[SerializeField] GameObject UICanvas;
	[SerializeField] GameObject districtIndicatorPrefab;
	Color[] districtColors;
	[SerializeField] Text districtHeader;
	[SerializeField] Text feedback;
	[SerializeField] Text instructions;
	[SerializeField] Text goalText;
	[SerializeField] Text stats;
    [SerializeField] Text clearCurrentDistrictButton;
    [SerializeField] TutorialUIManager tutorialPanel;

	//list of all the game spaces
	public List<County> allCounties = new List<County>();
    public County[,] gridCoordinates;

	//keeps track of how many people for each group in each district.
	//e.g. districtMakeup[(int)PoliticalParty.Circle][0] is how many people are in group A in district 1.
	//	   districtMakeup[(int)PoltiicalParty.Triangle][0] is how many people are in group B in district 1.
	public Vector2[] districtMakeup;
	public int[] totalPopulation;
	//for user input and UI
	bool haveScrolled = false;
	bool isSelecting = false; //whether or not the mouse is down and you're selecting
	int currentDistrict = 0; //the district that you're selecting for
	[SerializeField] DistrictIndicator[] indicators;

	//for interfacing with the other 'scenes' like the voting phase
	levelManager LM;

	// Use this for initialization
	public void Init ()
    {
		LM = GameObject.FindGameObjectWithTag ("levelManager").GetComponent<levelManager>();
        numDistricts = LM.getNumDistricts ();
		districtColors = LM.getColors ();

        if(LM.currentLevel.Contains("Tutorial1"))
        {
            tutorialPanel = GameObject.Find("TutorialPanel").GetComponent<TutorialUIManager>();
            //tutorialPanel.gameObject.SetActive(false); 
        }

        //start keeping track of the districts.
        districtMakeup = new Vector2[numDistricts];
		totalPopulation = new int[]{0,0};
        GameObject indicatorHolder = GameObject.Find("IndicatorHolder");
		indicators = new DistrictIndicator[numDistricts];
        mostPopulated = 0;
        leastPopulated = 0;
		for (int i = 0; i < numDistricts; i++)
        {
            districtMakeup[i] = Vector2.zero;

            GameObject indicator = Instantiate (districtIndicatorPrefab);
            indicator.transform.position = new Vector3 (50f, 225f + (75f * (numDistricts - 1)) - 75f * i, 0f);
			indicator.transform.SetParent (indicatorHolder.transform, false);
            indicator.name = "Indicator " + (i + 1);
			indicator.GetComponent<DistrictIndicator>().SetLabel(districtColors[i], (i + 1).ToString());
			indicators[i] = indicator.GetComponent<DistrictIndicator>();
			indicators [i].setDistrictMap(this);
			indicators [i].setDistructNumber(i+1);
		}

		float height = indicators [0].GetComponent<RectTransform> ().rect.height;
		height *= 0.9f * UICanvas.GetComponent<Canvas> ().scaleFactor;
		districtHeader.gameObject.transform.position = new Vector3 (indicators[0].transform.position.x,indicators[0].transform.position.y+height, 0f);
        rows = (int)TransitionData.Instance.dimensions.x;
        cols = (int)TransitionData.Instance.dimensions.y;

        gridCoordinates = new County[rows, cols];

        indicators [0].SetActive (true);

		goalText.text = "GOAL: "+ LM.getInstructions ();

		feedback.text = "";
//        clearCurrentDistrictButton = GameObject.Find("ClearCurrentDistrictText").GetComponent<Text>();
//        clearCurrentDistrictButton.text = "Clear " + (currentDistrict + 1);
        Services.EventManager.Register<KeyPressed>(OnKeyPressed);
    }

    private void SetIndicators()
    {
        for(int i = 0; i < numDistricts; i++)
        {
            indicators[i] = GameObject.Find("Indicator " + (i + 1)).GetComponent<DistrictIndicator>();
        }
        Services.EventManager.Register<KeyPressed>(OnKeyPressed);
    }

    private void OnDisable()
    {
        Services.EventManager.Unregister<KeyPressed>(OnKeyPressed);
    }

    private void OnDestroy()
    {
        Services.EventManager.Unregister<KeyPressed>(OnKeyPressed);
    }

    private void OnKeyPressed(KeyPressed key)
    {
        if(key.code == KeyCode.P)
        {
            ClearAll();
        }

        if(key.code == KeyCode.L)
        {
            ClearCurrentDistrict();
        }

        NextDistrict(key.code);
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
        allCounties.Add(county);
    }

    public County GetCounty(Vector2 pos)
    {
        foreach(County county in allCounties)
        {
            if (county.gridPos == pos)
                return county;
        }
        return null;
    }

    public bool CoordIsWithinBounds(Vector2 coord)
    {
        return coord.x >= 0 && coord.x < cols && coord.y >= 0 && coord.y < rows;
    }

    public int CountiesInDirstrict(int district)
    {
        int numOfCounties = 0;
        foreach(County county in allCounties)
        {
            if (county.getDistrict() == district)
            {
                numOfCounties++;
            }
        }

        return numOfCounties;
    }

    public void setCountyPopulation()
    {
        int totalCircle = 0;
        int totalTriangle = 0;
        foreach (County county in allCounties)
        {
            totalCircle += county.getCirclePatyPopulation();
            totalTriangle += county.getTrianglePartyPopulation();
        }

        int[] population = new int[] { totalCircle, totalTriangle };



        totalPopulation[(int)PoliticalParty.CIRCLE] = population[(int)PoliticalParty.CIRCLE];
		totalPopulation[(int)PoliticalParty.TRIANGLE] = population[(int)PoliticalParty.TRIANGLE];

		stats.text = "" + totalPopulation [(int)PoliticalParty.CIRCLE];
		stats.text += "\n" + totalPopulation [(int)PoliticalParty.TRIANGLE];

		LM.setTotalPopulation (totalPopulation);
		goalText.text = LM.getInstructions ();
    }

    public void SetMaxPopulationDifference()
    {
        //  Display this on the screen
        MAX_POPULATION_DIFFERENCE = Mathf.RoundToInt((totalPopulation[(int)PoliticalParty.CIRCLE] + totalPopulation[(int)PoliticalParty.TRIANGLE]) * 0.05f) + 1;
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

	public int GetDistrictCircleCounty(int districtNumber){
		int population = 0;
		foreach (County county in allCounties)
		{
			if (county.getDistrict () == districtNumber) {
				if (county.getGroup () == 0) {
					population += 1;
				}
			}
		}
		return population;
	}
	public int GetDistrictTriangleCounty(int districtNumber){
		int population = 0;
		foreach (County county in allCounties)
		{
			if (county.getDistrict () == districtNumber) {
				if (county.getGroup () == 1) {
					population += 1;
				}
			}
		}
		return population;
	}

    public int GetDistrictTotalPopulation(int districtNumber)
    {
        //  Otherwise we add all the members of each party
        //  in the district we are counting to its population
        int population = 0;
        foreach (County county in allCounties)
        {
            if (county.getDistrict() == districtNumber)
                population += county.getTotalPopulation();
        }
        return population;
    }

    public int GetDistrictCirclePopulation(int districtNumber)
    {
        int population = 0;
        foreach (County county in allCounties)
        {
            if (county.getDistrict() == districtNumber)
                population += county.getCirclePatyPopulation();
        }
        return population;
    }

    public int GetDistrictTrianglePopulation(int districtNumber)
    {
        int population = 0;
        foreach (County county in allCounties)
        {
            if (county.getDistrict() == districtNumber)
                population += county.getTrianglePartyPopulation();
        }
        return population;
    }

    public bool PopulationsAreEqual()
    {
        int totalCircle = 0;
        int totalTriangle = 0;
        foreach (County county in allCounties)
        {
            totalCircle += county.getCirclePatyPopulation();
            totalTriangle += county.getTrianglePartyPopulation();
        }

        return totalCircle == totalTriangle;
    }

    public bool AllDistrictsContinuityCheck()
    {
        bool[] districtChecks = new bool[numDistricts];
        for(int i = 0; i< districtChecks.Length; i++)
        {
            districtChecks[i] = false;
        }

        County[] firstInsance = new County[numDistricts];
        foreach(County county in allCounties)
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
        if (IsDistrictEmpty(currentDistrict)) return true;
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

        for (int i = 0; i < numDistricts; i++)
        {
            int population = GetDistrictTotalPopulation(i);
            if(population < lowestPopulation)
            {
                leastPopulated = i;
                lowestPopulation = population;
            }
            if(highestPopulation < population)
            {
                mostPopulated = i;
                highestPopulation = population;
            }
        }
        return highestPopulation - lowestPopulation <= MAX_POPULATION_DIFFERENCE;
    }

    void ValidatePopulationDistributionForUI()
    {
        if (!PopulationDistributionIsValid())
        {
            //  Makes most populated district number smaller and gray
            indicators[mostPopulated].SetTotalPopulationText(GetDistrictTotalPopulation(mostPopulated), Mathf.RoundToInt(indicators[mostPopulated].defaultPopFontSize * 0.80f), Color.gray);
            //  Makes the least populated district number bigger and red
            indicators[leastPopulated].SetTotalPopulationText(GetDistrictTotalPopulation(leastPopulated), Mathf.RoundToInt(indicators[leastPopulated].defaultPopFontSize * 1.66f), new Color (1.0f, 0.5f, 0.0f));
        }
        else
        {
            for (int i = 0; i < numDistricts; i++)
            {
                indicators[i].SetTotalPopulationText(GetDistrictTotalPopulation(i));
            }
        }
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

        ValidatePopulationDistributionForUI();

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
				
                //ok now set the district and increase the count
                // check if continuos
				int objectPreviousDistrict = objectHit.getDistrict();

                if (IsContinuous(objectHit))
                {

                    objectHit.setDistrict(currentDistrict);
                    if (AllDistrictsContinuityCheck())
                    {
                        if (prevDistrict != -1)
                        {
                            //it was previously a district, but now it's not gonna be that anymore
                            //so we lower the count
                            districtMakeup[currentDistrict] = new Vector2(GetDistrictCirclePopulation(currentDistrict) - objectHit.getCirclePatyPopulation(),
                                                                        GetDistrictTrianglePopulation(currentDistrict) - objectHit.getTrianglePartyPopulation());
                        }

                        objectHit.setColor(districtColors[currentDistrict]);
                        districtMakeup[currentDistrict] = new Vector2(districtMakeup[currentDistrict].x + objectHit.getCirclePatyPopulation(),
                                                                        districtMakeup[currentDistrict].y + objectHit.getTrianglePartyPopulation());
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

    public void ClearAll()
    {
        for(int i = 0; i < numDistricts; i++)
        {
            districtMakeup[i] = Vector2.zero;
        }

        foreach(County county in allCounties)
        {
            if(county.getDistrict() != -1)
            {
                county.setColor(new Color(1.0f, 1.0f, 1.0f, 0.0f));
                county.setDistrict(-1);
            }
        }

        UpdatePopulations();
    }

    public void ClearCurrentDistrict()
    {

        districtMakeup[currentDistrict] = Vector2.zero;
        

        foreach (County county in allCounties)
        {
            if (county.getDistrict() == currentDistrict)
            {
                county.setColor(new Color(1.0f, 1.0f, 1.0f, 0.0f));
                county.setDistrict(-1);
            }
        }

        UpdatePopulations();
    }

	public void ClearDistrict(int d){
		Debug.Log ("clear district " + d);
        districtMakeup[d] = Vector2.zero;

		foreach (County county in allCounties)
		{
			if (county.getDistrict() == d)
			{
				county.setColor(new Color(1.0f, 1.0f, 1.0f, 0.0f));
				county.setDistrict(-1);
			}
		}

		UpdatePopulations();
	}

	//finished redistricting
	public void Submit()
    {
		feedback.text = "";
		bool allSpacesAssigned = true;
		for (int i = 0; i < allCounties.Count; i++)
		{
			if (allCounties [i] != null) {
				if (allCounties [i].getDistrict () == -1)
                {
					feedback.color = Color.red;
					feedback.text = "Some people have not been assigned a district.";

                    Debug.Log ("Some people have not been assined a district");
					Debug.Log (allCounties [i].gridPos.x + ", " + allCounties [i].gridPos.y);

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
				feedback.text += "The population isn't distributed!";
				Debug.Log("One district has too many people");
			}
		}
	}

	void Analyze()
    {
		LM.setDistrictMakeup (districtMakeup);
		LM.setTotalPopulation (totalPopulation);
		int[] dp = new int[numDistricts];
		for (int i = 0; i < numDistricts; i++) {
			dp [i] = GetDistrictTotalPopulation (i);
		}
		LM.setDistrictPopulations (dp);

		transform.parent.GetComponent<PrototypeSceneScript> ().ChangeScene ();
	}

	public void NextDistrict(int d){
		NextDistrict (KeyCode.Alpha0 + d);
	}

    void NextDistrict(KeyCode kcode)
    {
        if (!kcode.ToString().Contains("Alpha")) return;

        int code = int.Parse(kcode.ToString().Replace("Alpha", ""));
        if (code == 0) return;

        indicators[currentDistrict].SetActive(false);
        for (int i = 0; i < numDistricts + 1; i++)
        {
            if (i == code)
            {
                currentDistrict = i - 1; 
            }
        }

        indicators[currentDistrict].SetActive(true);
//        clearCurrentDistrictButton.text = "Clear " + (currentDistrict + 1);
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
//        clearCurrentDistrictButton.text = "Clear " + (currentDistrict + 1);
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

                allCounties.Add (newCounty);

                gridCoordinates[i, j] = newCounty.GetComponent<County>();

                totalPopulation [(int)PoliticalParty.CIRCLE] += newCountyCirclePopulation;
				totalPopulation [(int)PoliticalParty.TRIANGLE] += (totalCountyPopulation - newCountyCirclePopulation);
			}
		}
		stats.text = "Circle population is: " + totalPopulation [(int)PoliticalParty.CIRCLE];
		stats.text += "\nTriangle population is: " + totalPopulation [(int)PoliticalParty.TRIANGLE];
	}
}
