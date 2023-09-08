using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MasterMoneyText : MonoBehaviour
{
    public TextMeshProUGUI mastermoney;
    // Update is called once per frame
    void Update()
    {
        mastermoney.text = "Current Money: " + GameManager.Instance._data.masterDatabase.masterData.money + "$";
    }
}
