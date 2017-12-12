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
	[SerializeField] GameObject indicatorHolder;

	[SerializeField] Text populationData;
	[SerializeField] Text resultText;
	[SerializeField] Image resultReaction;
	[SerializeField] Text[] feedbackPercentageText;
	[SerializeField] Text[] feedbackDistrictsText;
	[SerializeField] Text goalText;
	[SerializeField] Text detailedFeedback;

	levelManager LM;

	int[] totalPopulation; //0th index is for group 1, 1st index is for group 2
	Vector2[] districtMakeup;
    [SerializeField] int numDistricts;

	[SerializeField] int[] districtCount;

	bool isResultGood;

	Color[] districtColors;
	int[] districtPopulation;

	void Start(){
		isResultGood = false;
		LM = GameObject.FindGameObjectWithTag ("levelManager").GetComponent<levelManager>();
		totalPopulation = LM.getTotalPopulation ();
		districtMakeup = LM.getDistrictMakeup ();
		numDistricts = LM.getNumDistricts ();
		districtPopulation = LM.getDistrictPopulations ();

		score (LM.getScoreType ());
		districtColors = LM.getColors ();

		populationData.text = totalPopulation [0] + "\n" + totalPopulation [1];
		goalText.text = LM.getInstructions ();

		for (int i = 0; i < numDistricts; i++)
		{

			GameObject indicator = Instantiate (districtDataPrefab);

			indicator.transform.position = new Vector3 (50f, 225f + (75f * (numDistricts - 1)) - 75f * i, 0f);
			indicator.transform.SetParent (indicatorHolder.transform, false);
			DistrictData indicatorData = indicator.GetComponent<DistrictData>();
			indicatorData.setDistrict(districtColors[i],i+1);

			//this is bad code i am sorry
			if (districtMakeup[i].x > districtMakeup[i].y) {
				indicatorData.setParty (0);
			} else if (districtMakeup[i].x < districtMakeup[i].y) {
				indicatorData.setParty (1);
			} else {
				indicatorData.setParty (2);
			}

			indicatorData.setPopulation (districtPopulation[i]);
		}

		//hardcoded--again bad programming sorry
		//but also not using it sooo
		districtHeader.gameObject.transform.position = new Vector3 (50f,150+75f*(numDistricts-1)+90f, 0f);
	}

	public void score(int i){
		districtCount = getDistrictCount ();

		float[] ratio = groupPercentages ();
		feedbackPercentageText[0].text = "" + Mathf.Round(ratio[0]*100) + "%";
		feedbackPercentageText[1].text = "" + Mathf.Round(ratio[1]*100) + "%";
		feedbackDistrictsText[0].text = "" + districtCount[0]+" districts";
		feedbackDistrictsText[1].text = "" + districtCount[1]+" districts";

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
			resultText.text = "Nice job!";
			resultReaction.sprite = resultReactions [0];

            string levelText = TransitionData.Instance.lvl.ToUpper();
			if (levelText.Equals("TUTORIAL1"))
            {
				Debug.Log ("setting tutorial finished as true");
                Services.GameManager.SetFinishedTutorial1(true);
            }
            if (levelText.Equals("TUTORIAL2"))
            {
                Debug.Log("setting tutorial finished as true");
                Services.GameManager.SetFinishedTutorial2(true);
            }
            if (levelText.Equals("TUTORIAL3"))
            {
                Debug.Log("setting tutorial finished as true");
                Services.GameManager.SetFinishedTutorial3(true);
            }

            foreach (EasyLevels level in Enum.GetValues(typeof(EasyLevels)))
            {
                string levelString = level.ToString();
                if (levelString.Equals(levelText))
                {
                    Services.GameManager.completedEasyLevels[level] = true;
                }
            }

		} else {
			resultReaction.sprite = resultReactions [1];
			resultText.text = "Try again?";
		}
	}

	int[] getDistrictCount(){
		int group1=0;
		int group2=0;
		int tie = 0;
		for (int i = 0; i < numDistricts; i++) {
            if (districtMakeup[i].x > districtMakeup[i].y) {
                Debug.Log("District " + (i + 1) + ": " + districtMakeup[i]);
                group1++;
			} else if (districtMakeup[i].x < districtMakeup[i].y) {
				group2++;
			} else {
				tie++;
			}
		}
		return new int[]{group1, group2};
	}

	float[] groupPercentages(){

		float group1distribution = (float)totalPopulation [0] / (float)(totalPopulation [0] + totalPopulation [1]);
		float group2distribution = (float)totalPopulation [1] / (float)(totalPopulation [0] + totalPopulation [1]);
		return new float[]{ group1distribution, group2distribution };
	}

	bool isWellRepresented(){
		float[] ratios = groupPercentages();
		float group1distribution = ratios[0];
		float group2distribution = ratios[1];
		//round it to the closest % with the numdistricts
		//this is how many districts they SHOULD get
		int group1district = (int)Mathf.Round(group1distribution * numDistricts);
        int group2district = numDistricts - group1district;


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

//		if (pop_minority == 0) {
//			detailedFeedback.text = "Circle should've won more districts than Triangle.";
//		} else {
//			detailedFeedback.text = "Triangle should've won more districts than Circle.";
//		}
		return (result_minority != pop_minority);
	}

	//if the majority wins one more district than it "should" have (if possible)
	bool isMajorityLandslide(){
		float pop_majority = (float)Mathf.Max(totalPopulation[0],totalPopulation[1]) / (float)(totalPopulation [0] + totalPopulation [1]);
		pop_majority = Mathf.Round(pop_majority * numDistricts);

		int result_majority = Mathf.Max (districtCount [0], districtCount [1]);

//		feedbackText.text += "\nthe majority should have gotten " + pop_majority + " representatives,\nthey got " + result_majority + " representatives.";

//		if (totalPopulation [0] > totalPopulation [1]) {
//			detailedFeedback.text = "Circle should have gotten 3 districts.";
//		} else if (totalPopulation [0] < totalPopulation [1]) {
//			detailedFeedback.text = "Triangle should have gotten 3 districts.";
//		} else {
//			detailedFeedback.text = "The populations were equal! No majority.";
//		}

		if (pop_majority >= numDistricts) {
			return (result_majority == pop_majority);
		}

		return (result_majority > pop_majority);
	}
}
