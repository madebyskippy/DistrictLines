using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScoreType {WellRepresented, MinorityWin, MajorityLandslide};

public class levelManager : MonoBehaviour {

	/*
	 * keeps track of what each level is
	 * holds data so map phase and voting phase can share information
	 */

	int scoreType;
	int map;
	int numDistricts;

	int[] totalPopulation; //0th index is for group 1, 1st index is for group 2
	int[][] districtMakeup;

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
	public void setLevel(int st, int m, int nd){
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
		switch (scoreType) {
		case 0:
			instruc = "Represent the population accurately.";
			break;
		case 1:
			instruc = "Make sure the minority group gets more districts.";
			break;
		case 2:
			instruc = "Have the majority get more districts than it deserves.";
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

	public void setDistrictMakeup(int[][] dm){
		districtMakeup = dm;
	}

	public int[] getTotalPopulation(){
		return totalPopulation;
	}

	public int[][] getDistrictMakeup(){
		return districtMakeup;
	}
}
