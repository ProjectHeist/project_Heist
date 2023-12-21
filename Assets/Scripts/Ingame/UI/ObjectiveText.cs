using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Logics;

public class ObjectiveText : MonoBehaviour
{
    public TextMeshProUGUI obj;
    // Update is called once per frame
    void Update()
    {
        if (IngameManager.Instance.ObjectiveCleared)
        {
            obj.text = "Objective Cleared!\nGo to Extraction Point";
        }
        else
        {
            obj.text = "Obtain 20000$";
        }
    }
}
