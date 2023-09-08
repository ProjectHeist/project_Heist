using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrepareManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static PrepareManager instance = null;
    public bool canStart = false;
    public int totalplayernum;
    public int[] playerIndexes;
    public int[] weaponIndexes;
    public GameObject popup;
    public Transform playerContent;
    public Transform weaponContent;
    public int addPlayerTo = -1;
    public int addWeaponTo = -1;
    public List<GameObject> prefabs;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }
    public static PrepareManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    public void prepareMission()
    {
        totalplayernum = 2;
        playerIndexes = new int[totalplayernum];
        weaponIndexes = new int[totalplayernum];
        GameManager gm = GameManager.Instance;

        PlayerStat[] totalplayer = gm._data.playerDatabase.totalPlayerStat.playerStats;
        List<PlayerStat> currentplayers = new List<PlayerStat>();
        for (int i = 0; i < gm._data.masterDatabase.masterData.playerNumbers.Length; i++)
        {
            int idx = gm._data.masterDatabase.masterData.playerNumbers[i];
            currentplayers.Add(totalplayer[idx]);
        }
        WeaponStat[] totalweapons = gm._data.weaponDatabase.weaponStatList.weaponStats;
        List<WeaponStat> currentweapons = new List<WeaponStat>();
        for (int i = 0; i < gm._data.masterDatabase.masterData.weaponNumbers.Length; i++)
        {
            int idx = gm._data.masterDatabase.masterData.weaponNumbers[i];
            currentweapons.Add(totalweapons[idx]);
        }

        float yPosition = 115;
        for (int i = 0; i < currentplayers.Count; i++)
        {
            GameObject btn = Instantiate(prefabs[2]);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = currentplayers[i].playerName;
            btn.GetComponent<Button>().onClick.AddListener(() => { AddPlayer(); });
            btn.transform.SetPositionAndRotation(new Vector3(0.0f, yPosition, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            btn.transform.SetParent(playerContent, false);
            yPosition -= 35;
        }
        yPosition = 115;
        for (int i = 0; i < currentweapons.Count; i++)
        {
            GameObject btn = Instantiate(prefabs[3]);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = currentweapons[i].weaponName;
            btn.GetComponent<Button>().onClick.AddListener(() => { AddPlayer(); });
            btn.transform.SetPositionAndRotation(new Vector3(0.0f, yPosition, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            btn.transform.SetParent(weaponContent, false);
            yPosition -= 35;
        }
    }

    public void AddPlayer()
    {

    }

    public void AddWeapon()
    {

    }
}
