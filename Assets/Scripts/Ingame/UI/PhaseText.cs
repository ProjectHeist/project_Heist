using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhaseText : MonoBehaviour
{
    // Update is called once per frame
    public TextMeshProUGUI phaseText;
    void Update()
    {
        phaseText.text = "Current Phase: " + IngameManager.Instance.phase;
    }
}
