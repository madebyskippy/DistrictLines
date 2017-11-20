using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistrictIndicator : MonoBehaviour
{

	[SerializeField] GameObject label;
	[SerializeField] Text labeltext;
	[SerializeField] Text poptext;
	[SerializeField] GameObject[] groups;
	[SerializeField] Button labelButton;

    [SerializeField] public int totalPopulation { get; private set; }

	bool isActive;

	// Use this for initialization
	void Start ()
    {
		groups [(int)PoliticalParty.CIRCLE].transform.localScale = Vector3.zero;
		groups [(int)PoliticalParty.TRIANGLE].transform.localScale = Vector3.zero;
		isActive = false;
	}

	public void SetLabel(Color c, string l)
    {
		label.GetComponent<Image>().color = c;
		labeltext.text = l;
		labelButton.image.color = c;
		labelButton.transform.GetChild (0).GetComponent<Text> ().text = l;
	}

	public void SetActive(bool a)
    {
		isActive = a;
		if (isActive) {
			labelButton.transform.localScale = new Vector3(1.75f,1f,1f);
			labeltext.color = Color.white;
			labeltext.fontStyle = FontStyle.Bold;
			labelButton.transform.GetChild (0).transform.localScale = new Vector3 (0.57f, 1f, 1f);
		} else {
			labelButton.transform.localScale = Vector3.one;
			labeltext.color = Color.black;
			labeltext.fontStyle = FontStyle.Normal;
		}
	}

	public void SetPopulationBar(float districtCirclePopulation, float disrictTrianglePopulation)
    {
		float total = districtCirclePopulation + disrictTrianglePopulation;
        float ratio = districtCirclePopulation / total;

        if (total == 0)
        { 
			groups [(int)PoliticalParty.CIRCLE].transform.localScale = Vector3.zero;
			groups [(int)PoliticalParty.TRIANGLE].transform.localScale = Vector3.zero;
		} else {
            
            groups [(int)PoliticalParty.CIRCLE].transform.localScale = new Vector3 (ratio, 1f, 1f);
			groups [(int)PoliticalParty.TRIANGLE].transform.localScale = new Vector3 (1f, 1f, 1f);
		}
	}

	public void SetTotalPopulationText(int p)
    {
		poptext.text = p.ToString ();
	}
}
