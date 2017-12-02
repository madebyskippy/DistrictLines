using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TutorialUIManager manager;
    [SerializeField] private string tutorialText;

    private void Start()
    {
        manager = GameObject.Find("TutorialPanel").GetComponent<TutorialUIManager>();
    }

    public string GetText()
    {
        return tutorialText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Show");
        manager.ShowTutorialPanel(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        manager.ShowTutorialPanel(null);
    }
}
