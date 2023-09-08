using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyText : MonoBehaviour
{
    public TextMeshProUGUI moneytext;

    // Update is called once per frame
    void Update()
    {
        moneytext.text = "Current Money: " + IngameManager.Instance.TotalMoney + "$";
    }
}
