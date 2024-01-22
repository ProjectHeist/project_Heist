using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ingame;

[CreateAssetMenu(fileName = "PlayerDatabase", menuName = "Database/PlayerDatabase", order = 5)]
public class PlayerDatabase : ScriptableObject
{
    public PlayerStat[] playerStatList;
}

