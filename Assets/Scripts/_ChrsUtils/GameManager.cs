using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public bool savePreviousScene { get; private set; }

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
        NumPlayers = 1;
        _mainCamera = Camera.main;
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
