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
            ValidateMap();
            Services.GameManager.SaveScene();
        }
    }

    internal override void OnExit()
    {

    }

    private void ValidateMap()
    {
        if (districtMap.PopulationsAreEqual())
        {
            for (int x = 0; x < TransitionData.Instance.dimensions.x; x++)
            {
                for (int y = 0; y < TransitionData.Instance.dimensions.y; y++)
                {
                    Vector2 pos = new Vector2(x, y);

                    if (districtMap.GetCounty(pos).getTotalPopulation() < 3)
                    {
                        districtMap.GetCounty(pos).setCirclePartyPopulation
                            (districtMap.GetCounty(pos).getCirclePatyPopulation() + 1
                            );
                        return;
                    }
                }
            }
        }
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
