using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class districtIndicator : MonoBehaviour {

	[SerializeField] GameObject label;
	[SerializeField] Text labeltext;
	[SerializeField] Text poptext;
	[SerializeField] GameObject[] groups;

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
		label.GetComponent<Image>().color = c;
		labeltext.text = l;
	}

	public void setActive(bool a){
		isActive = a;
		if (isActive) {
			label.transform.localScale = new Vector3(1.75f,1f,1f);
			labeltext.color = Color.white;
			labeltext.fontStyle = FontStyle.Bold;
		} else {
			label.transform.localScale = Vector3.one;
			labeltext.color = Color.black;
			labeltext.fontStyle = FontStyle.Normal;
		}
	}

	public void setGroups(float levelA, float levelB){
		float total = levelA + levelB;
        float ratio = levelA / total;
        if (total == 0) { 
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
