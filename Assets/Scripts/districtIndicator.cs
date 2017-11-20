using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class districtIndicator : MonoBehaviour {

	[SerializeField] GameObject label;
	[SerializeField] Text labeltext;
	[SerializeField] Text poptext;
	[SerializeField] GameObject[] groups;
	[SerializeField] Button labelButton;

	bool isActive;

	// Use this for initialization
	void Start () {
		groups [0].transform.localScale = Vector3.zero;
		groups [1].transform.localScale = Vector3.zero;
		isActive = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setLabel(Color c, string l){
		labeltext.text = l;
		labelButton.image.color = c;
		labelButton.transform.GetChild (0).GetComponent<Text> ().text = l;
	}

	public void setActive(bool a){
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

	public void setGroups(float levelA, float levelB){
		float total = levelA+levelB;
		float ratio = (float)(levelA) / total;
		if (total == 0) {
			ratio = 0;
			groups [0].transform.localScale = Vector3.zero;
			groups [1].transform.localScale = Vector3.zero;
		} else {
			groups [0].transform.localScale = new Vector3 (ratio, 1f, 1f);
			groups [1].transform.localScale = new Vector3 (1f, 1f, 1f);
		}
	}

	public void setPopulation(int p){
		poptext.text = p.ToString ();
	}
}
