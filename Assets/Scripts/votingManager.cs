using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class votingManager : MonoBehaviour {

	[SerializeField] Text resultText;

	levelManager LM;

	int[] totalPopulation; //0th index is for group 1, 1st index is for group 2
	int[][] districtMakeup;

	bool isResultGood;

	void Start(){
		isResultGood = false;
		LM = GameObject.FindGameObjectWithTag ("levelManager").GetComponent<levelManager>();
		totalPopulation = LM.getTotalPopulation ();
		districtMakeup = LM.getDistrictMakeup ();

		score (LM.getScoreType ());
	}

	public void score(int i){
		switch (i) {
		case 1:
			isResultGood = isWellRepresented ();
			break;
		case 2:
			isResultGood = isMinorityWins ();
			break;
		case 3:
			isResultGood = isMajorityLandslide ();
			break;
		}

		if (isResultGood) {
			resultText.text = "good job!";
		} else {
			resultText.text = "oof, you didn't meet the goal.";
		}
	}

	bool isWellRepresented(){
		return false;
	}

	bool isMinorityWins(){
		return false;
	}

	bool isMajorityLandslide(){
		return false;
	}
}
