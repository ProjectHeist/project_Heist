using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public PlayerDatabase playerDatabase = new PlayerDatabase();
    public MapDatabase mapDatabase = new MapDatabase();
    public WeaponDatabase weaponDatabase = new WeaponDatabase();
    public MasterDatabase masterDatabase = new MasterDatabase();
    public TagList tagList;

    public void init()
    {
        Load();
        LoadData();
    }

    public void Load()
    {
        mapDatabase.Load();
        masterDatabase.Load();
    }
    public void LoadData()
    {
        var LoadedJson = Resources.Load<TextAsset>("Data/Map/TagList");
        Debug.Log(LoadedJson.text);
        tagList = JsonUtility.FromJson<TagList>(LoadedJson.text);
        Debug.Log(tagList.tag[0]);
    }
}

public class TagList
{
    public string[] tag;
}