using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridSpace : MonoBehaviour {

    private int district;
    public int party;

    public Vector2 gridPos;

	[SerializeField] SpriteRenderer districtSprite;
	[SerializeField] SpriteRenderer selectorSprite;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setGridPos(float x, float y)
    {
        setGridPos(new Vector3(x, 0, y));
    }

    public void setGridPos(Vector2 newPos)
    {
        setGridPos(new Vector3(newPos.x, 0, newPos.y));     
    }

    public void setGridPos(Vector3 newPos)
    {
        gridPos = new Vector2(newPos.x, newPos.z);
        transform.position = newPos;
    }

	public void setGroup(int g, Color c){
		party = g;
		districtSprite.color = c;
	}public int getGroup(){
		return party;
	}


	public void setDistrict(int d){
		district = d;
	}public int getDistrict(){
		return district;
	}

    public void setColor(Color color)
    {
        selectorSprite.color = color;
    }
}
