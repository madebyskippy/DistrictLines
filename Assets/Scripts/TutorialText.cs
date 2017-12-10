using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TutorialUIManager manager;
	GameObject display;
    [SerializeField] private string tutorialText;

	bool isActive;

	bool hasShown;

    private void Start()
    {
		hasShown = false;
		manager = null;
		isActive = false;
		try{
			manager = GameObject.Find("TutorialPanel").GetComponent<TutorialUIManager>();
		}catch(System.Exception ex){
			//do nothing
		}
		if (manager != null) {
			isActive = true;
		}
		display = transform.GetChild (0).gameObject;
		display.SetActive(false);
    }

    public string GetText()
    {
        return tutorialText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
		if (isActive) {
			if (!hasShown) {
				manager.ShowTutorialPanel (this);
			}
		}
    }

    public void OnPointerExit(PointerEventData eventData)
	{
		if (isActive) {
		}
    }

	public void openTutorial(){
		display.SetActive (true);
	}

	public void setHasShown(bool hs){
		hasShown = hs;
	}

	public void closeTutorial(){
		GetComponent<Image> ().raycastTarget = false;
		display.SetActive (false);
		manager.ShowTutorialPanel (null);
	}
}
