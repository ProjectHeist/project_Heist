using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionButton : MonoBehaviour
{
    public void LoadMission()
    {
        SceneManager.LoadScene("MissionScene");
    }
}
