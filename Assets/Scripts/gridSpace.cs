using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridSpace : MonoBehaviour {

	int district;
	int party;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setGroup(int g){
		party = g;
	}public int getGroup(){
		return party;
	}

	public void setDistrict(int d){
		district = d;
	}public int getDistrict(){
		return district;
	}
}
