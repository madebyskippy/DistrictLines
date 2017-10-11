using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class districtIndicator : MonoBehaviour {

	[SerializeField] GameObject label;
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

	public void setLabel(Color c){
		label.GetComponent<Image>().color = c;
	}

	public void setActive(bool a){
		isActive = a;
		if (isActive) {
			label.transform.localScale = Vector3.one * 1.5f;
		} else {
			label.transform.localScale = Vector3.one;
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
}
