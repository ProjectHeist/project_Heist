using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;

public class PhaseEndButton : MonoBehaviour
{
    public void EndPhase()
    {
        if (!IngameManager.Instance.isEnemyPhase)
            IngameManager.Instance.StartCoroutine(IngameManager.Instance.EnemyPhase());
    }
}
