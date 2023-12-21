using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ingame;

public class EXDatabase
{
    public List<PlayerEX> playerEXs;
    public PlayerEX[] EXs;
    public void Load()
    {
        EXs = Resources.LoadAll<PlayerEX>("Data/PlayerEX");
        for (int i = 0; i < EXs.Length; i++)
        {
            playerEXs.Add(EXs[i]);
        }

    }
}
