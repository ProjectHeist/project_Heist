using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Logics;

public class MasterMoneyText : MonoBehaviour
{
    public TextMeshProUGUI mastermoney;
    // Update is called once per frame
    void Update()
    {
        mastermoney.text = "Current Money: " + GameManager.Instance.currentMaster.money + "$";
    }
}
