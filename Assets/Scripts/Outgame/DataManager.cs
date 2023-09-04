using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public MapData testData;
    public MasterData masterData;

    public void init()
    {
        LoadMap();
        LoadData();
    }

    public void LoadMap()
    {
        var loadedJson = Resources.Load<TextAsset>("Data/Map/TestMap1");
        testData = JsonUtility.FromJson<MapData>(loadedJson.text);
    }
    public void SaveData()
    {

    }
    public void LoadData()
    {
        var loadedJson = Resources.Load<TextAsset>("Data/Master/TestMasterData");
        masterData = JsonUtility.FromJson<MasterData>(loadedJson.text);
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

[System.Serializable]
public class MasterData
{
    public int money;
    public int[] levels;
    public int stage;
    public string playerName;
}