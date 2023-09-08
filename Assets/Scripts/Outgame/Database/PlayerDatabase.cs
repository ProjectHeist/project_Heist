using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDatabase
{
    public PlayerStatList totalPlayerStat; // 전체 플레이어 스탯 리스트

    public void Load()
    {
        var LoadedJson = Resources.Load<TextAsset>("Data/Player/TestStat");
        totalPlayerStat = JsonUtility.FromJson<PlayerStatList>(LoadedJson.text);
        Debug.Log(totalPlayerStat.playerStats[0].playerName);
    }
}

[System.Serializable]
public class PlayerStatList
{
    public PlayerStat[] playerStats;
}
