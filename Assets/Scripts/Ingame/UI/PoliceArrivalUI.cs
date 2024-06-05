using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Logics;

public class PoliceArrivalUI : MonoBehaviour
{
    public TextMeshProUGUI turnText;
    void Update()
    {
        if (IngameManager.Instance.spawner != null && IngameManager.Instance.spawner.spawnTime != -1)
            turnText.text = "Police Arrival: " + IngameManager.Instance.spawner.spawnTime + " Turns Left";
        else
            turnText.text = "";
    }
}
