using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MasterDatabase
{
    public List<MasterData> masterDatas;
    private static string SavePath => Application.persistentDataPath + "/saves/";

    public void Save(string saveFileName)
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }
        string saveJson = JsonUtility.ToJson(GameManager.Instance.currentMaster);
        string saveFilePath = SavePath + saveFileName + ".json";
        File.WriteAllText(saveFilePath, saveJson);
    }

    public MasterData LoadData(string saveFileName)
    {
        string saveFilePath = SavePath + saveFileName + ".json";
        if (!File.Exists(saveFilePath))
        {
            return null;
        }
        string saveFile = File.ReadAllText(saveFilePath);
        MasterData masterData = JsonUtility.FromJson<MasterData>(saveFile);
        return masterData;
    }
}

[System.Serializable]
public class MasterData
{
    public MasterData()
    {
        money = 10000;
        levels = new int[] { 1, 1, 1 };
        stage = 0;
        playerNumbers = new List<int>();
        weaponNumbers = new List<int>();
    }
    public int index;
    public int money;
    public int[] levels;
    public int stage;
    public List<int> playerNumbers;
    public List<int> weaponNumbers;
    public string masterName;
}
