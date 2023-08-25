using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    TextAsset textAsset;
    public MapData testData;
    public void init()
    {
        LoadData();
    }

    public void LoadMap()
    {

    }
    public void SaveData()
    {

    }
    public void LoadData()
    {
        var loadedJson = Resources.Load<TextAsset>("Data/Map/TestMap1");
        testData = JsonUtility.FromJson<MapData>(loadedJson.text);
        Debug.Log(testData.inwalkable);
        Debug.Log(testData.height);
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

[System.Serializable]
public class MapData
{
    public int width;
    public int height;
    public GridVector[] inwalkable;
}

[System.Serializable]
public class GridVector
{
    public int x;
    public int y;
}