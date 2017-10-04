using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class districtMap : MonoBehaviour {

	//each grid space represented by a cube
	[SerializeField] GameObject cube;

	//the materials that represent the two groups
	[SerializeField] Material[] colors;

	[SerializeField] int numDistricts;

	//starting out with a square map of a set size
	//this is a serializefield so that we can make adjustments as we prototype.
	[SerializeField] int rows;
	[SerializeField] int cols;

	//UI stuff
	[SerializeField] GameObject UICanvas;
	[SerializeField] GameObject districtIndicatorPrefab;
	[SerializeField] Color[] districtColors;

	//list of all the game spaces
	List<GameObject> gridspaces;

	//keeps track of how many people for each group in each district.
	//e.g. districtMakeup[0][0] is how many people are in group A in district 1.
	//	   districtMakeup[1][0] is how many people are in group B in district 1.
	int[][] districtMakeup;

	//for user input and UI
	bool isSelecting = false; //whether or not the mouse is down and you're selecting
	int currentDistrict = -1; //the district that you're selecting for
	districtIndicator[] indicators;

	// Use this for initialization
	void Start () {
		//start keeping track of the districts.
		districtMakeup = new int[][]{new int[numDistricts], new int[numDistricts]};
		indicators = new districtIndicator[numDistricts];
		for (int i = 0; i < numDistricts; i++) {
			districtMakeup[0] [i] = 0;
			districtMakeup[1] [i] = 0;
			GameObject indicator = Instantiate (districtIndicatorPrefab);
			indicator.transform.SetParent (UICanvas.transform);
			indicator.transform.localPosition = new Vector3 (0f, 15f+30f*i, 0f);
			indicator.GetComponent<districtIndicator> ().setLabel (districtColors [i]);
			indicators [i] = indicator.GetComponent<districtIndicator>();
		}

		gridspaces = new List<GameObject> ();
		setup ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		}	


		//mouse input
		if (Input.GetMouseButtonDown (0)) {
			isSelecting = true;
			currentDistrict = (currentDistrict + 1) % numDistricts;
		}
		if (isSelecting && Input.GetMouseButton (0)) {
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
				GameObject objectHit = hit.transform.gameObject;
				objectHit.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = districtColors[currentDistrict];
				int prevDistrict = objectHit.GetComponent<gridSpace> ().getDistrict ();
				if (prevDistrict != -1) {
					districtMakeup [objectHit.GetComponent<gridSpace> ().getGroup ()] [prevDistrict]--;
				}
				objectHit.GetComponent<gridSpace> ().setDistrict (currentDistrict);
				districtMakeup [objectHit.GetComponent<gridSpace> ().getGroup ()] [currentDistrict]++;
			}
		}
		if (Input.GetMouseButtonUp (0)) {
			isSelecting = false;
			for (int i = 0; i < numDistricts; i++) {
				indicators [i].setGroups (districtMakeup [0] [i],districtMakeup [1] [i]);
			}

		}

	}

	//sets up the grid of the map
	void setup(){
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
				GameObject space = Instantiate (cube, new Vector3(rows*0.5f-i,0,cols*0.5f-j), Quaternion.identity);
				int g = Random.Range (0, colors.Length);
				space.GetComponent<MeshRenderer> ().material = colors [g];
				space.GetComponent<gridSpace> ().setGroup (g);
				space.GetComponent<gridSpace> ().setDistrict (-1);
				gridspaces.Add (space);
			}
		}
	}
}
