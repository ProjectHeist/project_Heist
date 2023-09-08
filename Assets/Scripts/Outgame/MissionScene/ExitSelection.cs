using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSelection : MonoBehaviour
{
    public GameObject popup;
    public void OnexitClicked()
    {
        popup.SetActive(false);
    }
}
