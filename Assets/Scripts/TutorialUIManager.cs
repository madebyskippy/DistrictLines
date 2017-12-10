using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{
    private Text tutorialText;
    private TutorialText currentHoverSpot;
	private Image tutorialPanel;
	private Image tutorialPanelShadow;
    private List<TutorialText> hoverSpots;

	TutorialText currentTutorial;

	bool isStart; //for whether it's the starting one that's visible when you just open up the page and not dependant on hover

	// Use this for initialization
	void Start ()
    {
        

//		tutorialPanel = GetComponent<Image>();
		isStart = true;
        tutorialText = GetComponentInChildren<Text>();

        GameObject[] hoverSpotsGameObjects = GameObject.FindGameObjectsWithTag("HoverSpot");
        hoverSpots = new List<TutorialText>();
        foreach (GameObject hoverSpot in hoverSpotsGameObjects)
        {
            hoverSpots.Add(hoverSpot.GetComponent<TutorialText>());
        }

        if (!TransitionData.Instance.lvl.Contains("Tutorial1"))
        {
            isStart = false;
            gameObject.SetActive(false);
            foreach(TutorialText hoverspot in hoverSpots)
            {
                hoverspot.gameObject.SetActive(false);
            }
        }

    }

    private void SetTutorialText(string text)
    {
        tutorialText.text = text;
    }

    public void ShowTutorialPanel(TutorialText hoverSpot)
    {
        if (hoverSpot != null)
        {
			isStart = false;
			if (currentTutorial == null) {
				transform.localScale = Vector3.one;
				SetTutorialText (hoverSpot.GetText ());
				currentTutorial = hoverSpot;
				currentTutorial.setHasShown (true);
				currentTutorial.openTutorial();
			}
        }
        else
        {
			transform.localScale = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

	public void CloseBox(){
		if (currentTutorial != null) {
			currentTutorial.closeTutorial ();
			currentTutorial = null;
		}

		if (isStart) {
			isStart = false;
			ShowTutorialPanel (null);
		}
	}
}
