using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AccuracyUp", fileName = "AccUpEX")]
public class AccUpEX : PlayerEX
{
    public float originalAcc;
    public override void UseEX(PlayerState ps)
    {
        if (duration != 0)
        {
            originalAcc = ps.accuracy;
            ps.accuracy = 1.0f;
        }
        else
        {
            ps.accuracy = originalAcc;
        }
    }
}
