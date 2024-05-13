using System.Collections;
using System.Collections.Generic;
using Ingame;
using Logics;
using TMPro;
using UnityEngine;

public class SuspicionUI : MonoBehaviour
{
    // Update is called once per frame
    public TextMeshProUGUI sus;
    void Update()
    {
        if (IngameManager.Instance.playerController.currentPlayer != null)
        {
            int idx = gameObject.GetComponentInParent<EnemyBehaviour>().enemyIndex;
            sus.text = IngameManager.Instance.playerController.currentPlayer.GetComponent<PlayerState>().suspicion[idx].ToString();
        }
        else
            sus.text = "";
    }
}
