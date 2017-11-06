using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelSelectSceneScript : Scene<TransitionData>
{

    private const string NO_LEVEL_SELECTED = "";
    private Vector2 NO_DIMENSION_SELECTED = Vector2.zero;

    [SerializeField] private KeyCode submit = KeyCode.Return;

    [SerializeField] private string selectedLevel = NO_LEVEL_SELECTED;
    [SerializeField] private Vector2 selectedDimensions;
    [SerializeField] private Text feedback;
	[SerializeField] private ScoreType scoreType;

    internal override void OnEnter(TransitionData data)
    {
        selectedDimensions = NO_DIMENSION_SELECTED;
        feedback = GameObject.Find("FeedbackText").GetComponent<Text>();
        feedback.text = "";
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

			GameObject.FindGameObjectWithTag ("levelManager").GetComponent<levelManager>().setLevel((int)scoreType,0,5);
            Services.Scenes.Swap<PrototypeSceneScript>();
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