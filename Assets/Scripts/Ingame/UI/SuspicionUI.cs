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
            int idx = IngameManager.Instance.playerController.currentPlayer.GetComponent<PlayerState>().playerIndex;
            sus.text = gameObject.GetComponentInParent<EnemyState>().suspicion[idx].ToString();
        }
        else
            sus.text = "";
    }
}
