using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "LobbyScene":
                    LobbyToTitle();
                    break;
                case "MissionScene":
                    MissionToLobby();
                    break;
                case "SampleScene":
                    IngameToLobby();
                    break;
            }
        }
    }

    void LobbyToTitle()
    {
        GameManager.Instance._data.SaveData();
        SceneManager.LoadScene("TitleScene");
    }

    void MissionToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    void IngameToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
