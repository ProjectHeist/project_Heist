using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEX : MonoBehaviour
{
    private PlayerState ps;
    public void UseEX()
    {
        ps = gameObject.GetComponent<PlayerState>();
    }
}
