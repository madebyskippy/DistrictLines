﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TutorialUIManager manager;
	GameObject display;
    [SerializeField] private string tutorialText;

    private void Start()
    {
        manager = GameObject.Find("TutorialPanel").GetComponent<TutorialUIManager>();
		display = transform.GetChild (0).gameObject;
		display.SetActive(false);
    }

    public string GetText()
    {
        return tutorialText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Show");
//		GetComponent<Image> ().color = new Color (1f,0.9f,0.36f,0.5f);
		display.SetActive(true);
        manager.ShowTutorialPanel(this);
    }

    public void OnPointerExit(PointerEventData eventData)
	{
		//		GetComponent<Image> ().color = new Color (1f,0.9f,0.36f,0.0f);
		display.SetActive(false);
        manager.ShowTutorialPanel(null);
    }
}
