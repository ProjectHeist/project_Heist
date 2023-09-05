using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public int value = 10000;

    public void OnMoneyInteracted()
    {
        Debug.Log(GameManager.Instance._data.masterDatabase.masterData.money);
        GameManager.Instance._data.masterDatabase.masterData.money += value;
        Debug.Log(GameManager.Instance._data.masterDatabase.masterData.money);
        Destroy(gameObject);
    }
}