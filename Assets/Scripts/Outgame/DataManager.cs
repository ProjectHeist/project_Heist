using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    TextAsset textAsset;
    public MapData testData;
    public void init()
    {

    }

    public void LoadMap()
    {

    }
    public void SaveData()
    {

    }
    public void LoadData()
    {

    }
}

public class CharacterData
{
    public int HP;
    public int moveRange;
    public int critRate;
    public float accuracy;
    public int hide;
    public int damage;
    public int attackRange;
}

public class MapData
{
    public int width;
    public int height;
    public int[][] inwalkable;
}