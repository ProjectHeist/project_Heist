using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager
{
    public PlayerDatabase playerDatabase = new PlayerDatabase();
    public MapDatabase mapDatabase = new MapDatabase();
    public WeaponDatabase weaponDatabase = new WeaponDatabase();
    public MasterDatabase masterDatabase = new MasterDatabase();
    public SaveDataInfo saveDataInfo;
    public TagList tagList;
    private static string SavePath => Application.persistentDataPath + "/saveInfo/";

    public void init()
    {
        Load();
        LoadData();
    }

    public void Load() // 불변하는 값들을 로드할 때
    {
        mapDatabase.Load();
        playerDatabase.Load();
        weaponDatabase.Load();
        var LoadedJson = Resources.Load<TextAsset>("Data/Map/TagList");
        tagList = JsonUtility.FromJson<TagList>(LoadedJson.text);
    }
    public void LoadData() //변하는 값들을 로드할 때 
    {
        masterDatabase.masterDatas = new List<MasterData>();
        LoadSaveInfo();
        for (int i = 0; i < saveDataInfo.Filenames.Count; i++)
        {
            masterDatabase.masterDatas.Add(masterDatabase.LoadData("SaveData" + i));
        }
    }

    public void SaveData() //변하는 값들을 세이브할 때 
    {
        UpdateSaveInfo();
        masterDatabase.Save("SaveData" + GameManager.Instance.currentMaster.index);
    }

    void LoadSaveInfo()
    {
        string saveFilePath = SavePath + "SaveFileInfo" + ".json";
        if (!File.Exists(saveFilePath))
        {
            saveDataInfo = new SaveDataInfo();
        }
        else
        {
            string saveFile = File.ReadAllText(saveFilePath);
            saveDataInfo = JsonUtility.FromJson<SaveDataInfo>(saveFile);
        }
    }

    void UpdateSaveInfo()
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }
        string saveJson = JsonUtility.ToJson(GameManager.Instance.currentMaster);
        string saveFilePath = SavePath + "SaveFileInfo" + ".json";
        File.WriteAllText(saveFilePath, saveJson);
    }
}

[System.Serializable]
public class SaveDataInfo
{
    public SaveDataInfo()
    {
        Filenames = new List<string>();
    }
    public List<string> Filenames;
}

public class TagList
{
    public string[] tag;
}