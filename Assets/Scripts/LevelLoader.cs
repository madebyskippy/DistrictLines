using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LevelLoader : MonoBehaviour
{
    public const string LEVEL_PATH = "";

    public Texture2D textureMap;


    private DistrictMap districtMap;

    public void setDistrictMap(DistrictMap map)
    {
        districtMap = map;
    }

	private void adjustCamera(Vector2 lvlDimension){
        if (lvlDimension.x < 8)
        {
            lvlDimension = new Vector2(8.0f, 3);
        }

		//it's camera size = 11 for 16x16 map
		Camera.main.orthographicSize = (int)(11 * (float)((float)lvlDimension.x/(float)16));

		//it's camera y position = 7 for 16x16 map
		//camera y position = 3.5 for 8x8 map
		//the y = mx + b is y = (3.5/8)x + 0
		Vector3 oldPos = Camera.main.transform.position;
		Camera.main.transform.position = new Vector3 (oldPos.x, oldPos.y, (float)(3.5f/8f)*lvlDimension.y + 0f);
	}

    public void loadLevel(string lvl, Vector2 lvlDimension)
    {
		adjustCamera (lvlDimension);
        Assert.IsNotNull(districtMap, "This district map has not been set for the Level Loader");

		textureMap = Resources.Load<Texture2D>("Levels/"+ lvlDimension.x.ToString() + "x" + lvlDimension.y.ToString()
                                        + "_" + lvl);

        if (lvl == "MAPTEST")
            lvlDimension = new Vector2(25, 25);

        Random.InitState(System.DateTime.Now.DayOfYear);

        for (int x = 0; x < lvlDimension.x; x++)
        {
            for (int y = 0; y < lvlDimension.y; y++)
            {
                if (textureMap.GetPixel(x, y) != Color.white)
                {
                    //  Do something or nothing
                    County space = Instantiate(districtMap.GetCountryPrefab(), new Vector3(x, 0, y), Quaternion.identity).GetComponent<County>();
                    space.transform.parent = Services.Scenes.CurrentScene.transform;
                    space.name = "County: " + x + ", " + y;
                    int totalInArea = Random.Range(1, 4); //total of 5 "people"
                    int firstGroup = Random.Range(0, totalInArea);
                    if (firstGroup == totalInArea / 2.0f)
                    {
                        //prevent ties, we can't handle them right now
                        firstGroup++;
                    }

                    space.setGroups(firstGroup, totalInArea - firstGroup);

                    space.setDistrict(-1);
                    space.setGridPos(x, y);

                    space.setCirclePartyPopulation(firstGroup);
                    space.setTrianglePartyPopulation((totalInArea - firstGroup));

                    districtMap.AddGridSpaceToMap(space);
                    districtMap.SetGridCoordinates(new Vector2(x, y), space);
                    districtMap.setCountyPopulation(new int[] { firstGroup, (totalInArea - firstGroup) });
                    districtMap.SetMaxPopulationDifference();
                }
                else
                {
                    //  Do something else or nothing
                }
            }
        }
    }
}
