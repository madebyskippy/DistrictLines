using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class districtIndicator : MonoBehaviour {

	[SerializeField] Image label;
	[SerializeField] GameObject[] groups;

	// Use this for initialization
	void Start () {
		groups [0].transform.localScale = Vector3.zero;
		groups [1].transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setLabel(Color c){
		label.color = c;
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
