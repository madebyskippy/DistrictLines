using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	// Use this for initialization
	void Start () {

		//TEMPORARY FOR TESTING! should be set by the level loader

		setLevel(Random.Range(1,4),0,5);

	}
	
	// Update is called once per frame
	void Update () {
		
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
		case 1:
			instruc = "make sure the final districts represent the population well!";
			break;
		case 2:
			instruc = "can you get the minority group to win?";
			break;
		case 3:
			instruc = "make sure the majority wins by a landslide.";
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
