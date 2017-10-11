using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridSpace : MonoBehaviour {

	int district;
	int party;

	[SerializeField] SpriteRenderer districtSprite;
	[SerializeField] SpriteRenderer selectorSprite;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setGroup(int g, Color c){
		party = g;
		districtSprite.color = c;
	}public int getGroup(){
		return party;
	}

	public void setDistrict(int d, Color c){
		district = d;
		selectorSprite.color = c;
	}public int getDistrict(){
		return district;
	}
}
