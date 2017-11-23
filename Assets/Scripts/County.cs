using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class County : MonoBehaviour {

    [SerializeField] private int district;
    private int[] partyCount;

    private int trianglePartyPopulation;
    private int circlePartyPopulation;

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
		int totalInCounty = partyCount [0] + partyCount [1];
		int cols = (int)Mathf.Ceil(Mathf.Sqrt(totalInCounty));
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < cols; j++) {
				if (i * cols + j < partyCount [0] + partyCount [1]) {
					//I could make a prefab but I just did all the gameobject settings here
					GameObject clump = new GameObject ();
					SpriteRenderer sr = clump.AddComponent<SpriteRenderer> ();
					sr.color = Color.grey;
					sr.sortingOrder = 1;
					clump.transform.parent = transform;
					clump.transform.localRotation = Quaternion.Euler (90f, 0f, 0f);
					if (i * cols + j >= partyCount [0]) {
						sr.sprite = populationClump [1];
						clump.transform.localScale = Vector3.one * 0.3f;
					} else {
						sr.sprite = populationClump [0];
						clump.transform.localScale = Vector3.one * 0.235f;
					}

					//center & grid it in the area
					if (totalInCounty == 1) {
						clump.transform.localPosition = Vector3.zero;
					} else if (totalInCounty == 2) {
						clump.transform.localPosition = new Vector3 (-0.175f + j*0.35f, 0f, 0f);
					} else if (totalInCounty == 3) {
						if (i * cols + j == 0) {
							//the first one
							clump.transform.localPosition = new Vector3 (0f, 0f, 0.175f);
						} else {
							clump.transform.localPosition = new Vector3 (-0.175f + j * 0.35f, 0f, -0.175f);
						}
					} else {
						//shouldn't happen but it's here just in case
						clump.transform.localPosition = new Vector3 (-0.175f * (cols - 1f) + i * 0.35f, 0f, -0.175f * (cols - 1f) + j * 0.35f);
					}

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

	public void setGroups(Vector2 groups)
    {
		partyCount[0] = (int)groups.x;
		partyCount[1] = (int)groups.y;
		displayClumps ();
	}

    public void setGroups(int g1, int g2)
    {
		partyCount[0] = g1;
		partyCount[1] = g2;
		displayClumps();
	}

    public int getGroup()
    {
		if (partyCount[0] > partyCount[1])
        {
			return 0;
		}
        else if (partyCount[0] < partyCount[1])
        {
			return 1;
		}
		return -1; //tie
	}


	public void setDistrict(int d)
    {
		district = d;
	}

    public int getDistrict()
    {
		return district;
	}

    public void setTrianglePartyPopulation(int population)
    {
        trianglePartyPopulation = population;
    }public int getTrianglePartyPopulation()
    {
        return trianglePartyPopulation;
    }

    public void setCirclePartyPopulation(int population)
    {
        circlePartyPopulation = population;
    }public int getCirclePatyPopulation()
    {
        return circlePartyPopulation;
    }

    public int getTotalPopulation()
    {
        return circlePartyPopulation + trianglePartyPopulation;
    }

    public void setColor(Color color)
    {
        selectorSprite.color = color;
    }
}
