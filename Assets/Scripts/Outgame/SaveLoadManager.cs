using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager
{
    public MasterData masterData;

    public void SaveData()
    {

    }
    public void LoadData()
    {
        var loadedJson = Resources.Load<TextAsset>("Data/Master/TestMasterData");
        masterData = JsonUtility.FromJson<MasterData>(loadedJson.text);
    }

}
