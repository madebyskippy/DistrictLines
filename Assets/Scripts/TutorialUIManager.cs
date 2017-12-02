using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{
    private Text tutorialText;
    private TutorialText currentHoverSpot;
    private Image tutorialPanel;
    private List<TutorialText> hoverSpots;
	// Use this for initialization
	void Start ()
    {
        tutorialPanel = GetComponent<Image>();
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
            tutorialPanel.color = new Color(0.38f, 0.38f, 0.38f, 1.0f);
            SetTutorialText(hoverSpot.GetText());
        }
        else
        {
            tutorialPanel.color = new Color(0.38f, 0.38f, 0.38f, 0.0f);
            SetTutorialText("");
        }
    }

    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << 5;
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log("Hit");
            currentHoverSpot = hit.transform.gameObject.GetComponent<TutorialText>();
            ShowTutorialPanel(currentHoverSpot);
        }

    }
}
