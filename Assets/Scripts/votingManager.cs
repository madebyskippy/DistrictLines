using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class votingManager : MonoBehaviour {
	
	[SerializeField] GameObject districtDataPrefab;
	[SerializeField] Sprite[] resultReactions;
	[SerializeField] GameObject districtHeader;
	[SerializeField] GameObject UICanvas;

	[SerializeField] Text populationData;
	[SerializeField] Text resultText;
	[SerializeField] Image resultReaction;
	[SerializeField] Text[] feedbackText;
	[SerializeField] Text goalText;

	levelManager LM;

	int[] totalPopulation; //0th index is for group 1, 1st index is for group 2
	int[][] districtMakeup;
    [SerializeField] int numDistricts;

	[SerializeField] int[] districtCount;

	bool isResultGood;

	Color[] districtColors;

	void Start(){
		isResultGood = false;
		LM = GameObject.FindGameObjectWithTag ("levelManager").GetComponent<levelManager>();
		totalPopulation = LM.getTotalPopulation ();
		districtMakeup = LM.getDistrictMakeup ();
		numDistricts = LM.getNumDistricts ();

		score (LM.getScoreType ());
		districtColors = LM.getColors ();

		populationData.text = totalPopulation [0] + "\n" + totalPopulation [1];
		goalText.text = "GOAL: "+ LM.getInstructions ();

		for (int i = 0; i < numDistricts; i++)
		{

			GameObject indicator = Instantiate (districtDataPrefab);

			indicator.transform.position = new Vector3 (50f, 150f + (75f * (numDistricts - 1)) - 75f * i, 0f);
			indicator.transform.SetParent (UICanvas.transform,false);
			DistrictData indicatorData = indicator.GetComponent<DistrictData>();
			indicatorData.setDistrict(districtColors[i],i+1);

			//this is bad code i am sorry
			if (districtMakeup [(int)PoliticalParty.CIRCLE] [i] > districtMakeup [(int)PoliticalParty.TRIANGLE] [i]) {
				indicatorData.setParty (0);
			} else if (districtMakeup [(int)PoliticalParty.CIRCLE] [i] < districtMakeup [(int)PoliticalParty.TRIANGLE] [i]) {
				indicatorData.setParty (1);
			} else {
				indicatorData.setParty (2);
			}

			indicatorData.setPopulation (districtMakeup [0][i] + districtMakeup [1][i]);
		}

		//hardcoded--again bad programming sorry
		districtHeader.gameObject.transform.position = new Vector3 (50f,150+75f*(numDistricts-1)+90f, 0f);
	}

	public void score(int i){
		districtCount = getDistrictCount ();

		switch (i) {
		case 0:
			isResultGood = isWellRepresented ();
			break;
		case 1:
			isResultGood = isMinorityWins ();
			break;
		case 2:
			isResultGood = isMajorityLandslide ();
			break;
		}

		if (isResultGood) {
			resultText.text = "Nice job reaching your goal!";
			resultReaction.sprite = resultReactions [0];

            string levelText = TransitionData.Instance.lvl.ToUpper();
            if (levelText.Equals("SQUARE"))
            {
                Services.GameManager.SetFinishedTutorial(true);
            }

            foreach(EasyLevels level in Enum.GetValues(typeof(EasyLevels)))
            {
                string levelString = level.ToString();
                if (levelString.Equals(levelText))
                {
                    Services.GameManager.completedEasyLevels[level] = true;
                }
            }

		} else {
			resultReaction.sprite = resultReactions [1];
			resultText.text = "Hmm, maybe try again?";
		}
	}

	int[] getDistrictCount(){
		int group1=0;
		int group2=0;
		int tie = 0;
		for (int i = 0; i < numDistricts; i++) {
            if (districtMakeup [(int)PoliticalParty.CIRCLE] [i] > districtMakeup [(int)PoliticalParty.TRIANGLE] [i]) {
				group1++;
			} else if (districtMakeup [(int)PoliticalParty.CIRCLE] [i] < districtMakeup [(int)PoliticalParty.TRIANGLE] [i]) {
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
        int group2district = numDistricts - group1district;


        feedbackText[0].text = "" + Mathf.Round(group1distribution*100) + "% of the population\nreceived "+districtCount[0]+" representatives.";
		feedbackText[1].text = "" + Mathf.Round(group2distribution*100) + "% of the population\nreceived "+districtCount[1]+" representatives.";

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
		feedbackText[0].text += "\nthe population minority was group " + pop_minority;
		feedbackText[1].text += "\nand the district representative minority is group " + result_minority+".";
		return (result_minority != pop_minority);
	}

	//if the majority wins one more district than it "should" have (if possible)
	bool isMajorityLandslide(){
		float pop_majority = (float)Mathf.Max(totalPopulation[0],totalPopulation[1]) / (float)(totalPopulation [0] + totalPopulation [1]);
		pop_majority = Mathf.Round(pop_majority * numDistricts);

		int result_majority = Mathf.Max (districtCount [0], districtCount [1]);

//		feedbackText.text += "\nthe majority should have gotten " + pop_majority + " representatives,\nthey got " + result_majority + " representatives.";
		if (pop_majority >= numDistricts) {
			return (result_majority == pop_majority);
		}

		return (result_majority > pop_majority);
	}
}
