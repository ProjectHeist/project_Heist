using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    bool canStart;
    public void StartMission()
    {
        canStart = false;
        for (int i = 0; i < PrepareManager.Instance.totalplayernum; i++)
        {
            if (PrepareManager.Instance.playerIndexes[i] != -1)
            {
                canStart = true;
            }
        }
        if (canStart)
        {
            GameManager.Instance.StartGame("SampleScene", PrepareManager.Instance.playerIndexes, PrepareManager.Instance.weaponIndexes);
        }
    }
}
