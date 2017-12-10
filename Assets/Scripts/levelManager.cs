using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScoreType {WellRepresented, MinorityWin, MajorityLandslide};

public class levelManager : MonoBehaviour {

	/*
	 * keeps track of what each level is
	 * holds data so map phase and voting phase can share information
	 */
    
    public string currentLevel { get; private set; }
	int scoreType;
	int map;
	int numDistricts;

	int[] totalPopulation = new int[2]; //0th index is for group 1, 1st index is for group 2
	Vector2[] districtMakeup;

	int[] districtPopulation;

	//the colors that represent the two groups
	[SerializeField] Color[] colors;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Color[] getColors(){
		return colors;
	}

	//-----
	//-----for map phase
	//-----
	public void setLevel(string _levelName, int st, int m, int nd){
        currentLevel = _levelName;
		scoreType = st;
		map = m;
		numDistricts = nd;
	}

	public int getNumDistricts(){
		return numDistricts;
	}

	public int getMap(){
		return map;
	}

	public int getScoreType(){
		return scoreType;
	}

	//for the map phase, so user knows what to aim for
	public string getInstructions(){
		string instruc = "";
		string majority = "Triangle";
		string minority = "Circle";
		if (totalPopulation [0] > totalPopulation [1]) {
			majority = "Circle";
			minority = "Triange";
		}
		float ratio = (float)totalPopulation [0] / (float)(totalPopulation [0] + totalPopulation [1]);
		switch (scoreType) {
		case 0:
			instruc = "Be fair!\n";
			int circDistrict = (int)Mathf.Round (ratio * 3f);
			Debug.Log ((ratio*3f)+","+circDistrict);
			if (circDistrict == 0 || circDistrict == 3) {
				instruc += majority + " should get 3 districts.";
			} else {
				instruc += majority+" should get 2 districts,\n";
				instruc += minority+" should get 1 district.";
			}
			break;
		case 1:
			instruc = "Turn the tables!\n";
			instruc += "Make sure "+minority+" gets 2 or more districts.";
			break;
		case 2:
			instruc = "Leave no openings.\n";
			instruc += "Make sure "+majority+" gets 3 districts.";
			break;
		}
		return instruc;
	}

	//-----
	//-----for end of map phase, to use in voting phase
	//-----
	public void setTotalPopulation(int[] tp){
		totalPopulation = tp;
	}

	public void setDistrictMakeup(Vector2[] dm){
		districtMakeup = dm;
	}

	public int[] getTotalPopulation(){
		return totalPopulation;
	}

	public Vector2[] getDistrictMakeup(){
		return districtMakeup;
	}

	public void setDistrictPopulations(int[] dp){
		districtPopulation = dp;
	}

	public int[] getDistrictPopulations(){
		return districtPopulation;
	}
}
