using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logics;

public class TurnText : MonoBehaviour
{
    // Update is called once per frame
    public TextMeshProUGUI turnText;
    void Update()
    {
        turnText.text = "Current Turn: " + IngameManager.Instance.turn;
    }
}
