using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDatabase
{
    public MapData testData;

    public void Load()
    {
        var loadedJson = Resources.Load<TextAsset>("Data/Map/TestMap1");
        testData = JsonUtility.FromJson<MapData>(loadedJson.text);
    }
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
