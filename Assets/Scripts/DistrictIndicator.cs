using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistrictIndicator : MonoBehaviour
{

	[SerializeField] GameObject label;
	[SerializeField] Text labeltext;
	[SerializeField] Text poptext;
	[SerializeField] GameObject[] groupBars;
	[SerializeField] GameObject[] groupLabels;
	[SerializeField] Button labelButton;
	[SerializeField] Image tieIndicator;

    [SerializeField] public int totalPopulation { get; private set; }

	private DistrictMap districtMap;

	int districtNumber;
	Color districtColor;

	bool isActive;

    public int defaultPopFontSize { get; private set; }
    private Color defaultColor;
	// Use this for initialization
	void Start ()
    {
        defaultPopFontSize = poptext.fontSize;
        defaultColor = poptext.color;
		groupBars [(int)PoliticalParty.CIRCLE].transform.localScale = Vector3.zero;
		groupBars [(int)PoliticalParty.TRIANGLE].transform.localScale = Vector3.zero;
		isActive = false;
	}

    private void OnDestroy()
    {
        Debug.Log("NOPE");
    }

    public void setDistrictMap(DistrictMap dm){
		districtMap = dm;
	}

	public void setDistructNumber(int i){
		districtNumber = i;
	}

	public void buttonClicked(){
		districtMap.NextDistrict (districtNumber);
	}

	public void clearClicked(){
		districtMap.ClearDistrict (districtNumber-1);
	}

	public void SetLabel(Color c, string l)
    {
		label.GetComponent<Image>().color = c;
		districtColor = c;
		labeltext.text = l;
		labelButton.image.color = c;
		labelButton.transform.GetChild (0).GetComponent<Text> ().text = l;
	}

	public void SetActive(bool a)
    {
		isActive = a;
		if (isActive) {
			labelButton.transform.localScale = new Vector3(1.6f,1f,1f);
			labeltext.color = Color.white;
			labelButton.transform.GetChild (0).transform.localScale = new Vector3 (0.57f, 1f, 1f);
		} else {
			labelButton.transform.localScale = Vector3.one;
			labeltext.color = Color.black;
		}
	}

	public void SetPopulationBar(float districtCirclePopulation, float disrictTrianglePopulation)
    {
		float total = districtCirclePopulation + disrictTrianglePopulation;
        float ratio = districtCirclePopulation / total;

        if (total == 0)
        { 
			groupBars [(int)PoliticalParty.CIRCLE].transform.localScale = Vector3.zero;
			groupBars [(int)PoliticalParty.TRIANGLE].transform.localScale = Vector3.zero;
			groupLabels [(int)PoliticalParty.CIRCLE].SetActive (false);
			groupLabels [(int)PoliticalParty.TRIANGLE].SetActive (false);
		} else {
			groupBars [(int)PoliticalParty.CIRCLE].transform.localScale = new Vector3 (ratio, 1f, 1f);
			groupBars [(int)PoliticalParty.TRIANGLE].transform.localScale = new Vector3 (1f, 1f, 1f);
			groupLabels [(int)PoliticalParty.CIRCLE].SetActive (true);
			groupLabels [(int)PoliticalParty.TRIANGLE].SetActive (true);

			//if there is 0 of a group
			if (ratio == 1) {
				groupLabels [(int)PoliticalParty.TRIANGLE].SetActive (false);
			} else if (ratio == 0) {
				groupLabels [(int)PoliticalParty.CIRCLE].SetActive (false);
			}

			//showing which group has the district
			int maj=0;
			int min=9;
			if (ratio > 0.5f) { //circle is majority
				maj = (int)PoliticalParty.CIRCLE;
				min = (int)PoliticalParty.TRIANGLE;
			} else if (ratio < 0.5f) { //triangle is majority
				maj = (int)PoliticalParty.TRIANGLE;
				min = (int)PoliticalParty.CIRCLE;
			} else {
				//tie
				tieIndicator.color = new Color (1,1,1, 1);
				groupLabels [(int)PoliticalParty.CIRCLE].GetComponent<Image> ().color = new Color(0.65f,0.65f,0.65f,1f);
				groupLabels [(int)PoliticalParty.TRIANGLE].GetComponent<Image> ().color = new Color(0.65f,0.65f,0.65f,1f);
				groupBars [(int)PoliticalParty.CIRCLE].GetComponent<Image> ().color = new Color(0.7f,0.7f,0.7f,1f);
				groupBars [(int)PoliticalParty.TRIANGLE].GetComponent<Image> ().color = new Color(0.7f,0.7f,0.7f,1f);
				return;
			}
			tieIndicator.color = new Color (0, 0, 0, 0);

			//convert district color to something opaque
			float districtH;
			float districtS;
			float districtV;
			Color.RGBToHSV (districtColor,out districtH,out districtS, out districtV);
			districtS = 0.33f;
			Color districtOpaque = Color.HSVToRGB (districtH, districtS, districtV);

			groupBars [maj].GetComponent<Image>().color = districtOpaque;
			groupBars [min].GetComponent<Image>().color = new Color(0.8f,0.8f,0.8f,1f);
			groupLabels [maj].GetComponent<Image> ().color = Color.white;
			groupLabels [min].GetComponent<Image> ().color = new Color(0.7f,0.7f,0.7f,1f);
		}
	}

    public void SetTotalPopulationText(int population)
    {
        SetTotalPopulationText(population, defaultPopFontSize, defaultColor);
    }

    public void SetTotalPopulationText(int population, int fontSize, Color color)
    {
        Debug.Log(poptext);
        if(poptext == null)
        {
            poptext = transform.Find("population count").Find("population text").GetComponent<Text>();
        }
		poptext.text = population.ToString ();
        poptext.fontSize = fontSize;
        poptext.color = color;
	}
}
