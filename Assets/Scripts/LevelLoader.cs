using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum Level
{
    TEXTURETEST, MAPTEST, CALIFORNIA_STATE, FLORIDA_STATE, OHIO_STATE, NEWYORK_NYC 
}

public class LevelLoader : MonoBehaviour
{
    public const string LEVEL_PATH = "";

    public Texture2D textureMap;


    private DistrictMap districtMap;

    public void setDistrictMap(DistrictMap map)
    {
        districtMap = map;
    }

    public void loadLevel(string lvl, Vector2 lvlDimension)
    {
        Assert.IsNotNull(districtMap, "This district map has not been set for the Level Loader");

        textureMap = Resources.Load<Texture2D>("Levels/"+ 16 + "x" + 16
                                        + "_" + lvl);

        if (lvl == "MAPTEST")
            lvlDimension = new Vector2(25, 25);

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
                    int totalInArea = Random.Range(1, 4); //total of 9 "people"
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
                }
                else
                {
                    //  Do something else or nothing
                    Debug.Log("Not White");
                }
            }
        }
    }
}
