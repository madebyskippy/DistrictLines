using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Level
{
    TEXTURETEST, MAPTEST 
}

public class LevelLoader : MonoBehaviour
{
    public const string LEVEL_PATH = "";

    public Texture2D map;


    public void loadLevel(Level lvl)
    {
        map = Resources.Load<Texture2D>("Levels/" + lvl.ToString());

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                if (map.GetPixel(x, y) == Color.white)
                {
                    //  Do something or nothing
                    Debug.Log("White");
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
