using UnityEngine;

public class IntroSceneScript : Scene<TransitionData>
{

	private const int LEFT_CLICK = 0;

	private const float SECONDS_TO_WAIT = 0.5f;

	private TaskManager _tm = new TaskManager();

	internal override void OnEnter(TransitionData data)
	{

	}

	internal override void OnExit()
	{

	}

	private void StartGame()
	{
		_tm.Do
		(
			new Wait(SECONDS_TO_WAIT))
			.Then(new ActionTask(ChangeScene)
			);
	}

	private void ChangeScene()
	{
		//Services.Scenes.Swap<GameSceneScript>();
		Services.Scenes.Swap<LevelSelectSceneScript>();
	}

	private void Update()
	{
		_tm.Update();
		//        if (Input.GetMouseButtonDown(LEFT_CLICK))
		//        {
		//            StartGame();
		//        }
	}

	public void StartButtonClick(){
		StartGame ();
	}
}
