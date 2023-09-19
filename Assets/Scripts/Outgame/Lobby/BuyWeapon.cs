using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class BuyWeapon : MonoBehaviour
{
    public GameObject weaponPopup;
    public GameObject weaponbtn;
    public Transform weaponcontents;
    public List<GameObject> weaponbtnlist;
    public TextMeshProUGUI infoText;
    public int weaponidx = -1;
    public void activatepopup()
    {
        WeaponStat[] ws = GameManager.Instance._data.weaponDatabase.weaponStatList.weaponStats;
        Debug.Log(ws.Length);
        weaponPopup.SetActive(true);
        float yPosition = 115;
        if (weaponbtnlist.Count == 0)
        {
            for (int i = 0; i < ws.Length; i++)
            {
                GameObject btn = Instantiate(weaponbtn);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = ws[i].weaponName;
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() => { onWeaponClicked(index); });
                btn.transform.SetPositionAndRotation(new Vector3(0.0f, yPosition, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                btn.transform.SetParent(weaponcontents, false);
                yPosition -= 35;
                weaponbtnlist.Add(btn);
            }
        }
    }

    public void onWeaponClicked(int idx)
    {
        weaponidx = idx;
        WeaponStat ws = GameManager.Instance._data.weaponDatabase.weaponStatList.weaponStats[idx];
        infoText.text = "이름: " + ws.weaponName + "\n가격: " + ws.weaponCost + "$" + "\n치명타 확률: " + ws.weaponCritRate * 100 + "%" + "\n사거리: " + ws.weaponMaxAttackRange + "\n명중 보장 사거리: " + ws.weaponMinAttackRange + "\n피해량: " + ws.weaponDamage;
    }

    public void OnBuyBtnClicked()
    {
        if (weaponidx != -1)
        {
            WeaponStat selected = GameManager.Instance._data.weaponDatabase.weaponStatList.weaponStats[weaponidx];
            if (selected.weaponCost <= GameManager.Instance.currentMaster.money)
            {
                GameManager.Instance.currentMaster.money -= selected.weaponCost;
                GameManager.Instance.currentMaster.weaponNumbers.Add(selected.weaponNumber);
                weaponidx = -1;
            }
        }
    }

    public void deactivatepopup()
    {
        weaponPopup.SetActive(false);
    }
}
