using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSelectButton : MonoBehaviour
{
    public GameObject popup;
    public void SelectMission()
    {
        popup.SetActive(true);
        PrepareManager.Instance.prepareMission();
    }
}