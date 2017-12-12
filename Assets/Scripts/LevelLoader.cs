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

        //it's camera y position = 7 for 16x16 map
        //camera y position = 3.5 for 8x8 map
        //the y = mx + b is y = (3.5/8)x + 0

        //it's camera x position = 0 for 16x16
        //camera x position = -2.5 for 3x3 map
		//the y = mx + b is y = (2.5 / (16-3))x + 16 * (2.5 / (16-3))

		Vector3 oldPos = Camera.main.transform.position;
		Camera.main.transform.position = new Vector3 (0.1923f*lvlDimension.x - 3.077f, oldPos.y, (float)(3.5f/8f)*lvlDimension.y + 0f);


        if (lvlDimension.x < 8)
        {
            lvlDimension = new Vector2(10.0f, 3);
        }
        else if (lvlDimension.x == 8)
        {
            lvlDimension = new Vector2(12.0f, 8);
        }
        else
        {
            lvlDimension = new Vector2(19.0f, 16);
        }


        


       

		//it's camera size = 11 for 16x16 map
		Camera.main.orthographicSize = (int)(11 * (float)((float)lvlDimension.x/(float)16));
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
                Color pixelColor = textureMap.GetPixel(x,y);
                if (pixelColor != Color.white)
                {
                    County space = Instantiate(districtMap.GetCountryPrefab(), new Vector3(x, 0, y), Quaternion.identity).GetComponent<County>();
                    space.transform.parent = Services.Scenes.CurrentScene.transform;
                    space.name = "County: " + x + ", " + y;
                    int totalInArea = 0;
                    int circlePopulation = 0;

                    
                    if (pixelColor == Color.red)
                    {
                        totalInArea = 1;
                        circlePopulation = 0;
                    }
                    else if (pixelColor == Color.green)
                    {
                        totalInArea = 1;
                        circlePopulation = 1;
                    }
                    else if (pixelColor == Color.blue)
                    {
                        totalInArea = 2;
                        circlePopulation = 0;
                    }
                    else if (pixelColor == Color.magenta)
                    {
                        totalInArea = 2;
                        circlePopulation = 2;
                    }
                    else if (pixelColor.r > 0.5f && pixelColor.g > 0.5f && pixelColor.b < 0.5f)
                    {
                        totalInArea = 3;
                        circlePopulation = 1;
                    }
                    else if (pixelColor == Color.cyan)
                    {
                        totalInArea = 3;
                        circlePopulation = 2;
                    }
                    else
                    {
                        totalInArea = Random.Range(1, 4); //total of 5 "people"
                        circlePopulation = Random.Range(0, totalInArea);
                        if (circlePopulation == totalInArea / 2.0f)
                        {
                            //prevent ties, we can't handle them right now
                            circlePopulation++;
                        }
                    }

                    space.setGroups(circlePopulation, totalInArea - circlePopulation);

                    space.setDistrict(-1);
                    space.setGridPos(x, y);

                    space.setCirclePartyPopulation(circlePopulation);
                    space.setTrianglePartyPopulation((totalInArea - circlePopulation));

                    districtMap.AddGridSpaceToMap(space);
                    districtMap.setCountyPopulation();
                    districtMap.SetGridCoordinates(new Vector2(x, y), space);
                    districtMap.SetMaxPopulationDifference();
                }
            }
        }

        if(districtMap.PopulationsAreEqual())
        {
            for (int x = 0; x < lvlDimension.x; x++)
            {
                for (int y = 0; y < lvlDimension.y; y++)
                {
                    Vector2 pos = new Vector2(x, y);
                    if(districtMap.gridCoordinates[x, y] != null && 
                       districtMap.gridCoordinates[x,y].getTotalPopulation() < 2)
                    {
                        int newTrianglePartyPopulation = 0;
                        int newCirclePopultion = 2;

                        
                        districtMap.gridCoordinates[x, y].setCirclePartyPopulation(0);
                        districtMap.gridCoordinates[x, y].setTrianglePartyPopulation(0);

                        districtMap.gridCoordinates[x, y].setGroups(0, 0);
                        Destroy(districtMap.gridCoordinates[x, y].gameObject.transform.Find("New Game Object").gameObject);


                        districtMap.gridCoordinates[x, y].setGroups(newCirclePopultion, newTrianglePartyPopulation);

                        districtMap.gridCoordinates[x, y].setCirclePartyPopulation(newCirclePopultion);
                        districtMap.gridCoordinates[x, y].setTrianglePartyPopulation(newTrianglePartyPopulation);
                        districtMap.setCountyPopulation();
                        return;
                    }
                }
            }
        }
    }
}
