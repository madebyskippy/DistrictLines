using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeSceneScript : Scene<TransitionData>
{
    //  Make branching architecture for people to choose the puzzele
    //  they want to solve
    //
    //  Chris: level loading/menu framework
    //
    //  ASCII level loader

    public DistrictMap districtMap;

    internal override void OnEnter(TransitionData data)
    {
        if (!Services.GameManager.savePreviousScene)
        {
            districtMap = GameObject.Find("DistrictMap").GetComponent<DistrictMap>();
            districtMap.Init();
            Services.LevelLoader.setDistrictMap(districtMap);
            Services.LevelLoader.loadLevel(TransitionData.Instance.lvl, TransitionData.Instance.dimensions);
            Services.GameManager.SaveScene();
        }
    }

    internal override void OnExit()
    {

    }
    public void ToLevelSelect()
    {
        Services.GameManager.PrepareToSaveScene();
        Services.Scenes.PopScene();
    }

    public void ChangeScene(){
		Services.Scenes.PushScene<VotingSceneScript>();
	}

    // Update is called once per frame
    void Update () {
		
	}
}
