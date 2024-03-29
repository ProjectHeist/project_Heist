using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Logics;
using Ingame;

public class BuyPlayer : MonoBehaviour
{
    public GameObject playerPopup;
    public GameObject playerbtn;
    public Transform playercontents;
    public List<GameObject> playerbtnlist;
    public TextMeshProUGUI infoText;
    public int playeridx = -1;
    public void activatepopup()
    {
        PlayerStat[] ps = GameManager.Instance._data.totalDB.playerDatabase.playerStatList;
        playerPopup.SetActive(true);
        float yPosition = 115;
        for (int i = 0; i < ps.Length; i++)
        {
            GameObject btn = Instantiate(playerbtn);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = ps[i].playerName;
            int index = i;
            btn.GetComponent<Button>().onClick.AddListener(() => { onPlayerClicked(index); });
            btn.transform.SetPositionAndRotation(new Vector3(0.0f, yPosition, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            btn.transform.SetParent(playercontents, false);
            yPosition -= 35;
            if (GameManager.Instance.currentMaster.playerNumbers.Contains(ps[i].playerNumber)) //갖고 있을 경우
            {
                btn.GetComponent<Button>().interactable = false;
            }
            playerbtnlist.Add(btn);
        }
    }

    public void onPlayerClicked(int idx)
    {
        playeridx = idx;
        PlayerStat ps = GameManager.Instance._data.totalDB.playerDatabase.playerStatList[idx];
        infoText.text = "이름: " + ps.playerName + "\n계약금: " + ps.playerCost + "$" + "\n분배 비율: " + ps.playerPercent * 100 + "%" + "\n체력: " + ps.playerHP + "\n명중률: " + ps.playerAccuracy * 100 + "%" + "\n이동 범위: " + ps.playerMoveRange;
    }

    public void OnBuyBtnClicked()
    {
        if (playeridx != -1)
        {
            PlayerStat selected = GameManager.Instance._data.totalDB.playerDatabase.playerStatList[playeridx];
            if (selected.playerCost <= GameManager.Instance.currentMaster.money)
            {
                GameManager.Instance.currentMaster.money -= selected.playerCost;
                GameManager.Instance.currentMaster.playerNumbers.Add(selected.playerNumber);
                playerbtnlist[playeridx].GetComponent<Button>().interactable = false;
                playeridx = -1;
            }
        }
    }

    public void deactivatepopup()
    {
        playerPopup.SetActive(false);
    }
}
