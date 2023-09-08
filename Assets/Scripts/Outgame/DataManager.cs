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
        playerDatabase.Load();
        weaponDatabase.Load();
    }
    public void LoadData()
    {
        var LoadedJson = Resources.Load<TextAsset>("Data/Map/TagList");
        tagList = JsonUtility.FromJson<TagList>(LoadedJson.text);
        masterDatabase.Load();
    }

    public void SaveData()
    {

    }
}

public class TagList
{
    public string[] tag;
}