using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public enum EasyLevels { RECTANGLE, CROSS, DONUT, L, CHECKER, CORNER}

public class GameManager : MonoBehaviour
{

    public bool savePreviousScene { get; private set; }
    public bool finishedTutorial { get; private set; }
    public Dictionary<EasyLevels, bool> completedEasyLevels;

    public KeyCode restartPrototype = KeyCode.R;

    [SerializeField] private int _numPlayers;
    public int NumPlayers
    {
        get { return _numPlayers; }
        private set
        {
            if (_numPlayers <= 0)
            {
                _numPlayers = 1;
            }
            else
            {
                _numPlayers = value;
            }
        }
    }

    [SerializeField] private Camera _mainCamera;
    public Camera MainCamera
    {
        get { return _mainCamera; }
    }

    public void Init()
    {
        completedEasyLevels = new Dictionary<EasyLevels, bool>();

        finishedTutorial = false;
        PopulateDictionary();
        NumPlayers = 1;
        _mainCamera = Camera.main;
    }

    private void PopulateDictionary()
    {
        foreach(EasyLevels level in Enum.GetValues(typeof(EasyLevels)))
        {
            completedEasyLevels.Add(level, false);
        }
    }

	// Use this for initialization
	public void Init (int players)
    {
        NumPlayers = players;
        _mainCamera = Camera.main;
	}
	
    public void ChangeCameraTo(Camera camera)
    {
        _mainCamera = camera;
    }

    private void RestartPrototype()
    {
        SceneManager.LoadScene("prototype");
    }

    public void SetFinishedTutorial(bool isFinished)
    {
        finishedTutorial = isFinished;
    }

    public void PrepareToSaveScene()
    {
        savePreviousScene = false;
    }

    public void SaveScene()
    {
        savePreviousScene = true;
    }

	// Update is called once per frame
	void Update ()
    {
        Services.InputManager.Update();

	    if(Input.GetKeyDown(restartPrototype))
        {
            RestartPrototype();
        }
	}
}
