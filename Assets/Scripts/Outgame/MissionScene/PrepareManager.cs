using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Ingame;
using Logics;

public class PrepareManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static PrepareManager instance = null;
    public bool canStart = false;
    public int totalplayernum;
    public int[] playerIndexes;
    public int[] weaponIndexes;
    public GameObject popup;
    public Transform SlotHolder;
    public Transform playerContent;
    public Transform weaponContent;
    public int addPlayerTo = -1;
    public int addWeaponTo = -1;
    public List<GameObject> prefabs;
    public List<GameObject> playerSlots;
    public List<GameObject> weaponSlots;
    public List<GameObject> playerList;
    public List<GameObject> weaponList;

    List<PlayerStat> currentplayers;
    List<WeaponStat> currentweapons;
    public Dictionary<int, int> playerMap = new Dictionary<int, int>(); // 앞쪽은 플레이어 보유중인 캐릭터 리스트 인덱스, 뒤쪽은 편성 슬롯 인덱스
    public Dictionary<int, int> weaponMap = new Dictionary<int, int>(); // 앞쪽은 플레이어 보유중인 무기 리스트 인덱스, 뒤쪽은 편성 슬롯 인덱스

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
        totalplayernum = GameManager.Instance._data.mapDatabase.testData.maxPlayerNum;
        playerIndexes = new int[totalplayernum];
        weaponIndexes = new int[totalplayernum];
        Array.Fill(playerIndexes, -1);
        Array.Fill(weaponIndexes, -1);
        GameManager gm = GameManager.Instance;

        PlayerStat[] totalplayer = gm._data.playerDatabase.totalPlayerStat.playerStats;
        currentplayers = new List<PlayerStat>(); // 현재 플레이어가 가지고 있는 캐릭터들
        for (int i = 0; i < gm.currentMaster.playerNumbers.Count; i++)
        {
            int idx = gm.currentMaster.playerNumbers[i];
            currentplayers.Add(totalplayer[idx]);
        }
        WeaponStat[] totalweapons = gm._data.weaponDatabase.weaponStatList.weaponStats;
        currentweapons = new List<WeaponStat>(); //현재 플레이어가 가지고 있는 무기들
        for (int i = 0; i < gm.currentMaster.weaponNumbers.Count; i++)
        {
            int idx = gm.currentMaster.weaponNumbers[i];
            currentweapons.Add(totalweapons[idx]);
        }

        float yPosition = 115;
        for (int i = 0; i < currentplayers.Count; i++) // scrollRect에 캐릭터 추가
        {
            GameObject btn = Instantiate(prefabs[2]);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = currentplayers[i].playerName;
            int index = i;
            btn.GetComponent<Button>().onClick.AddListener(() => { AddPlayer(index); });
            playerList.Add(btn);
            btn.transform.SetPositionAndRotation(new Vector3(0.0f, yPosition, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            btn.transform.SetParent(playerContent, false);
            yPosition -= 35;
        }
        yPosition = 115;
        for (int i = 0; i < currentweapons.Count; i++) //scrollRect에 무기 추가
        {
            GameObject btn = Instantiate(prefabs[3]);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = currentweapons[i].weaponName;
            int index = i;
            btn.GetComponent<Button>().onClick.AddListener(() => { AddWeapon(index); });
            weaponList.Add(btn);
            btn.transform.SetPositionAndRotation(new Vector3(0.0f, yPosition, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            btn.transform.SetParent(weaponContent, false);
            yPosition -= 35;
        }

        AddSlots();
    }

    public void AddSlots()
    {
        float xPosition = -250;
        for (int i = 0; i < totalplayernum; i++)
        {
            GameObject playerSlot = Instantiate(prefabs[0]);
            playerSlot.GetComponentInChildren<TextMeshProUGUI>().text = "Character " + (i + 1);
            playerSlot.GetComponent<PlayerSelect>().index = i;
            playerSlot.transform.SetPositionAndRotation(new Vector3(xPosition, 40.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            playerSlot.transform.SetParent(SlotHolder, false);
            playerSlots.Add(playerSlot);

            GameObject weaponSlot = Instantiate(prefabs[1]);
            weaponSlot.GetComponentInChildren<TextMeshProUGUI>().text = "Weapon " + (i + 1);
            weaponSlot.GetComponent<WeaponSelect>().index = i;
            weaponSlot.transform.SetPositionAndRotation(new Vector3(xPosition, -40.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            weaponSlot.transform.SetParent(SlotHolder, false);
            weaponSlots.Add(weaponSlot);
            xPosition += 80;
        }
    }

    public void AddPlayer(int i) // i가 현재 보유 캐릭터 리스트, addPlayerTo가 편성 슬롯
    {
        if (addPlayerTo != -1)
        {
            if (playerMap.ContainsKey(i))
            {
                int prev = playerMap[i]; // 딕셔너리와 연결하여 해당 플레이어를 사용 중인 슬롯 찾기
                playerSlots[prev].GetComponentInChildren<TextMeshProUGUI>().text = "Character " + (prev + 1); //표기 초기화
                playerIndexes[prev] = -1; // 원래 슬롯은 초기화 
                playerMap[i] = addPlayerTo; // 새 슬롯을 사용중인 것으로 딕셔너리 수정 
            }
            else
            {
                playerMap.Add(i, addPlayerTo); // 딕셔너리에 추가
            }
            playerIndexes[addPlayerTo] = currentplayers[i].playerNumber; // 출격 캐릭터 리스트에 해당 플레이어의 인덱스 부여
            playerSlots[addPlayerTo].GetComponentInChildren<TextMeshProUGUI>().text = currentplayers[i].playerName;
        }
    }

    public void AddWeapon(int i)
    {
        if (addWeaponTo != -1)
        {
            if (weaponMap.ContainsKey(i))
            {
                int prev = weaponMap[i];
                weaponSlots[prev].GetComponentInChildren<TextMeshProUGUI>().text = "Weapon " + (prev + 1);
                weaponIndexes[prev] = -1;
                weaponMap[i] = addWeaponTo;
            }
            else
            {
                weaponMap.Add(i, addWeaponTo);
            }
            weaponIndexes[addWeaponTo] = currentweapons[i].weaponNumber;
            weaponSlots[addWeaponTo].GetComponentInChildren<TextMeshProUGUI>().text = currentweapons[i].weaponName;
        }

    }
}
