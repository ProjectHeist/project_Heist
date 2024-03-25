using System.Collections;
using System.Collections.Generic;
using Ingame;
using TMPro;
using UnityEngine;

public class SuspicionUI : MonoBehaviour
{
    // Update is called once per frame
    public PlayerState ps;
    public TextMeshProUGUI sus;
    void Update()
    {
        sus.text = ps.suspicion.ToString();
    }
}
