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

	// Use this for initialization
	void Start ()
    {
//		tutorialPanel = GetComponent<Image>();
		tutorialPanel = transform.GetChild (1).GetComponent<Image> ();
		tutorialPanelShadow = transform.GetChild (0).GetComponent<Image> ();
        tutorialText = GetComponentInChildren<Text>();

        GameObject[] hoverSpotsGameObjects = GameObject.FindGameObjectsWithTag("HoverSpot");
        hoverSpots = new List<TutorialText>();
        foreach (GameObject hoverSpot in hoverSpotsGameObjects)
        {
            hoverSpots.Add(hoverSpot.GetComponent<TutorialText>());
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
	}
}
