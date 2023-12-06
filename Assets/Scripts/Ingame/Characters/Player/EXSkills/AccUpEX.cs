using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AccuracyUp", fileName = "AccUpEX")]
public class AccUpEX : PlayerEX
{
    public float originalAcc;
    public override void UseEX(PlayerState ps)
    {
        float diff = 1.0f - ps.accuracy;
        ps.accuracy = 1.0f;
        ps.StateChangeList.Add(new BuffInfo(stats.accuracy, duration, diff));
    }
}
