using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistrictData : MonoBehaviour {

	[SerializeField] Sprite[] partySprites;

	[SerializeField] Image party;
	[SerializeField] Text population;
	[SerializeField] Image districtLabel;
	[SerializeField] Text districtText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setDistrict(Color c, int d){
		districtText.text = d.ToString ();
		districtLabel.color = c;
	}

	public void setParty(int p){
		party.sprite = partySprites [p];
	}

	public void setPopulation(int pop){
		population.text = pop.ToString();
	}
}
