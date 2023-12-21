using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;

public class Money : MonoBehaviour
{
    public int value = 10000;

    public void OnMoneyInteracted()
    {
        Debug.Log(GameManager.Instance.currentMaster.money);
        IngameManager.Instance.TotalMoney += value;
        Debug.Log(GameManager.Instance.currentMaster.money);
        Destroy(gameObject);
    }
}
