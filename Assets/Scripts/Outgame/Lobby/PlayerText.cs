using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Logics;

public class PlayerText : MonoBehaviour
{
    public TextMeshProUGUI nametext;

    void Update()
    {
        nametext.text = "Player: " + GameManager.Instance.currentMaster.masterName;
    }
}
