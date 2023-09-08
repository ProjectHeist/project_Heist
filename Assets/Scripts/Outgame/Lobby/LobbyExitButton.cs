using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyExitButton : MonoBehaviour
{
    public void ExittoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
