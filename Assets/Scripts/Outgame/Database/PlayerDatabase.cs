using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDatabase
{
    public PlayerStatList totalPlayerStat; // 전체 플레이어 스탯 리스트

    public void Load()
    {

    }
}

public class PlayerStatList
{
    public PlayerStat[] playerStats;
}
