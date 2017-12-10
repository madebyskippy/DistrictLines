using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelSelectSceneScript : Scene<TransitionData>
{

    private const string NO_LEVEL_SELECTED = "";
    private Vector2 NO_DIMENSION_SELECTED = Vector2.zero;

    [SerializeField] private KeyCode submit = KeyCode.Return;

    [SerializeField] private int HARD_MODE_GATE;
    [SerializeField] private string selectedLevel = NO_LEVEL_SELECTED;
    [SerializeField] private Vector2 selectedDimensions;
    [SerializeField] private Text feedback;
	[SerializeField] private ScoreType scoreType;

    [SerializeField] private Button[] easyLevels;
    [SerializeField] private Button[] hardLevels;

	//for UI
	[SerializeField] private GameObject selectMapSquare;
	[SerializeField] private GameObject selectScoreSquare;

    internal override void OnEnter(TransitionData data)
    {
        //        selectedDimensions = NO_DIMENSION_SELECTED;

        //start it out with some defaults
        HARD_MODE_GATE = 3;
        easyLevels = PopulateButtonArray("Easy");
        hardLevels = PopulateButtonArray("Hard");

        setDimensions ("3");
		setLevel ("Tutorial1");
		setScoreType (0);

        feedback = GameObject.Find("FeedbackText").GetComponent<Text>();
        feedback.text = "";
		if (!Services.GameManager.finishedTutorial)
        {
            ToggleLevelButtons(easyLevels, false);
        }
        else
        {
            ToggleLevelButtons(easyLevels, true);
        }

        int easyLevelsFinished = 0;
        foreach(KeyValuePair<EasyLevels, bool> levels in Services.GameManager.completedEasyLevels)
        {
            if (levels.Value)
            {
                easyLevelsFinished++;
            }
        }

        if (easyLevelsFinished < HARD_MODE_GATE)
        {
            ToggleLevelButtons(hardLevels, false);
        }
        else
        {
            ToggleLevelButtons(hardLevels, true);
        }
        Services.GameManager.PrepareToSaveScene();
    }

    private void ToggleLevelButtons(Button[] buttonArray, bool interactable)
    {
        for (int i = 0; i < buttonArray.Length; i++)
        {
            buttonArray[i].interactable = interactable;
        }
    }

    private Button[] PopulateButtonArray(string tag)
    {
        GameObject[] buttonGameObjects = GameObject.FindGameObjectsWithTag(tag);
        Button[] buttonType = new Button[buttonGameObjects.Length];
        for (int i = 0; i < buttonGameObjects.Length; i++)
        {
            buttonType[i] = buttonGameObjects[i].GetComponent<Button>();
        }

        return buttonType;
    }

	public void moveMapSelector(Button b){
		selectMapSquare.transform.position = b.transform.position;
	}

	public void moveScoreSelector(Button b){
		selectScoreSquare.transform.position = b.transform.position;
	}

    public void setLevel(string lvl)
    {
        selectedLevel = lvl;
        TransitionData.Instance.lvl = lvl;
    }

    public void setDimensions(string dimensions)
    {
        int dim = int.Parse(dimensions);
        selectedDimensions = new Vector2(dim, dim);
        TransitionData.Instance.dimensions = selectedDimensions;
    }

	public void setScoreType(int type){
		scoreType = (ScoreType)type;
		TransitionData.Instance.scoreType = (ScoreType)type;

	}

    public void refreshFeedbackText()
    {
        feedback.text = "";
    }

    public void startGame()
    {
        if(selectedLevel == NO_LEVEL_SELECTED)
        {
            feedback.text = feedback.text + "Please Select a Level";
        }
        else if(selectedDimensions == NO_DIMENSION_SELECTED)
        {
            feedback.text = feedback.text + "\nPlease Select a Dimmension";
        }
        else
        {

			GameObject.FindGameObjectWithTag ("levelManager").GetComponent<levelManager>().setLevel(selectedLevel,(int)scoreType,0,3);
            Services.Scenes.PushScene<PrototypeSceneScript>();
        }

    }

    internal override void OnExit()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(submit))
        {
            startGame();
        }
    }
}