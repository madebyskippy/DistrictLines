using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class votingManager : MonoBehaviour {

	[SerializeField] Text resultText;
	[SerializeField] Text feedbackText;

	levelManager LM;

	int[] totalPopulation; //0th index is for group 1, 1st index is for group 2
	int[][] districtMakeup;
	int numDistricts;

	int[] districtCount;

	bool isResultGood;

	void Start(){
		isResultGood = false;
		LM = GameObject.FindGameObjectWithTag ("levelManager").GetComponent<levelManager>();
		totalPopulation = LM.getTotalPopulation ();
		districtMakeup = LM.getDistrictMakeup ();
		numDistricts = LM.getNumDistricts ();

		score (LM.getScoreType ());
	}

	public void score(int i){
		districtCount = getDistrictCount ();

		feedbackText.text = "group 1 had "+totalPopulation[0]+" people and group 2 had "+totalPopulation[1]+" people.\n";

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

	int[] getDistrictCount(){
		int group1=0;
		int group2=0;
		int tie = 0;
		for (int i = 0; i < numDistricts; i++) {
			if (districtMakeup [0] [i] > districtMakeup [1] [i]) {
				group1++;
			} else if (districtMakeup [0] [i] < districtMakeup [1] [i]) {
				group2++;
			} else {
				tie++;
			}
		}
		return new int[]{group1, group2};
	}

	bool isWellRepresented(){
		float group1distribution = (float)totalPopulation [0] / (float)(totalPopulation [0] + totalPopulation [1]);
		float group2distribution = (float)totalPopulation [1] / (float)(totalPopulation [0] + totalPopulation [1]);
		//round it to the closest % with the numdistricts
		//this is how many districts they SHOULD get
		int group1district = (int)Mathf.Floor(group1distribution * numDistricts);
		int group2district = (int)Mathf.Floor(group2distribution * numDistricts);

		feedbackText.text += "\ngroup 1 had " + Mathf.Round(group1distribution*100) + "% of the population and got "+districtCount[0]+" representatives.";
		feedbackText.text += "\ngroup 2 had " + Mathf.Round(group2distribution*100) + "% of the population and got "+districtCount[1]+" representatives.";

		if (group1district == districtCount [0]) {
			return true;
		}
		return false;
	}

	bool isMinorityWins(){
		int result_minority = 1;
		int pop_minority = 1;
		if (districtCount [0] < districtCount [1]) {
			result_minority = 0;
		}
		if (totalPopulation [0] < totalPopulation [1]) {
			pop_minority = 0;
		}
		feedbackText.text += "\nthe population minority was group " + pop_minority + " and the district representative minority is group " + result_minority+".";
		return (result_minority != pop_minority);
	}

	//if the majority wins one more district than it "should" have (if possible)
	bool isMajorityLandslide(){
		float pop_majority = (float)Mathf.Max(totalPopulation[0],totalPopulation[1]) / (float)(totalPopulation [0] + totalPopulation [1]);
		pop_majority = Mathf.Round(pop_majority * numDistricts);

		int result_majority = Mathf.Max (districtCount [0], districtCount [1]);

		feedbackText.text += "\nthe majority should have gotten " + pop_majority + " representatives, they got " + result_majority + " representatives.";
		if (pop_majority >= numDistricts) {
			return (result_majority == pop_majority);
		}

		return (result_majority > pop_majority);
	}
}
