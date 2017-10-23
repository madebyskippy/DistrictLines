using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridSpace : MonoBehaviour {

    private int district;
    private int[] partyCount;

    public Vector2 gridPos;

	[SerializeField] SpriteRenderer districtSprite;
	[SerializeField] SpriteRenderer selectorSprite;
	[SerializeField] Sprite[] populationClump;

	// Use this for initialization
	void Awake () {
		partyCount = new int[2];
	}

	void Start(){
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void displayClumps(){
		//ceiling the square root to get the cols/rows and then place them
		int cols = (int)Mathf.Ceil(Mathf.Sqrt(partyCount [0] + partyCount [1]));
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < cols; j++) {
				if (i * cols + j < partyCount [0] + partyCount [1]) {
					//I could make a prefab but I just did all the gameobject settings here
					GameObject clump = new GameObject ();
					SpriteRenderer sr = clump.AddComponent<SpriteRenderer> ();
					sr.color = Color.grey;
					sr.sortingOrder = 1;
					if (i * cols + j >= partyCount [0]) {
						sr.sprite = populationClump [1];
						clump.transform.localScale = Vector3.one * 0.23f;
					} else {
						sr.sprite = populationClump [0];
						clump.transform.localScale = Vector3.one * 0.18f;
					}
					clump.transform.parent = transform;
					clump.transform.localRotation = Quaternion.Euler (90f, 0f, 0f);

					//center & grid it in the area
					clump.transform.localPosition = new Vector3 (-0.125f * (cols - 1f) + i * 0.25f, 0f, -0.125f * (cols - 1f) + j * 0.25f);
				} else {
					break;
				}
			}
		}
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

	public void setGroups(Vector2 groups){
		partyCount[0] = (int)groups.x;
		partyCount[1] = (int)groups.y;
		displayClumps ();
	}public void setGroups(int g1, int g2){
		partyCount[0] = g1;
		partyCount[1] = g2;
		displayClumps();
	}public int getGroup(){
		if (partyCount[0] > partyCount[1]){
			return 0;
		}else if (partyCount[0] < partyCount[1]){
			return 1;
		}
		return -1; //tie
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
