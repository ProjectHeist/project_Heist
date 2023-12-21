using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Logics;

public class TitleManager : MonoBehaviour
{
    public GameObject loadPanel;
    public Transform MasterContent;
    public GameObject loadTable;
    public GameObject newgamePopup;
    public TMP_InputField userName;

    public void OnStartbtnClicked()
    {
        newgamePopup.SetActive(true);
    }

    public void startNewGame()
    {
        if (userName.text.Length > 0)
        {
            MasterData master = new MasterData();
            master.masterName = userName.text;
            master.index = GameManager.Instance._data.saveDataInfo.Filenames.Count; // 새 내용 생성

            GameManager.Instance.currentMaster = master;
            GameManager.Instance._data.masterDatabase.masterDatas.Add(master); // 리스트에 추가
            GameManager.Instance._data.SaveData();
            SceneManager.LoadScene("LobbyScene");
        }
    }

    public void dontStartNewGame()
    {
        newgamePopup.SetActive(false);
    }

    public void LoadPopup()
    {
        loadPanel.SetActive(true);
        List<MasterData> masterDatas = GameManager.Instance._data.masterDatabase.masterDatas;
        float yPosition = 115;
        for (int i = 0; i < masterDatas.Count; i++)
        {
            GameObject btn = Instantiate(loadTable);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = masterDatas[i].masterName + "\n" + masterDatas[i].money;
            int index = i;
            btn.GetComponent<Button>().onClick.AddListener(() => { OnSaveDataClicked(index); });
            btn.transform.SetPositionAndRotation(new Vector3(0.0f, yPosition, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            btn.transform.SetParent(MasterContent, false);
            yPosition -= 35;
        }
    }

    public void OnSaveDataClicked(int index)
    {
        GameManager.Instance.currentMaster = GameManager.Instance._data.masterDatabase.masterDatas[index];
        SceneManager.LoadScene("LobbyScene");
    }


    public void EscLoad()
    {
        loadPanel.SetActive(false);
    }
}
