using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXDatabase : MonoBehaviour
{
    public List<PlayerEX> playerEXs;
    public Object[] EXs;
    public void Load()
    {
        EXs = Resources.LoadAll("Data/PlayerEX");
        foreach (PlayerEX ex in EXs)
        {
            playerEXs.Add(ex);
            Debug.Log(ex.name);
        }

    }
}
