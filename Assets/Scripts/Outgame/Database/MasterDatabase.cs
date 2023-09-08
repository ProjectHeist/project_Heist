using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDatabase
{
    public MasterData masterData;

    public void Load()
    {
        var LoadedJson = Resources.Load<TextAsset>("Data/Master/TestMasterData");
        masterData = JsonUtility.FromJson<MasterData>(LoadedJson.text);
    }
}

[System.Serializable]
public class MasterData
{
    public int money;
    public int[] levels;
    public int stage;
    public int[] playerNumbers;
    public int[] weaponNumbers;
    public string masterName;
}
