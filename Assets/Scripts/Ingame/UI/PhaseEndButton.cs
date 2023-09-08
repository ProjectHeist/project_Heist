using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseEndButton : MonoBehaviour
{
    public void EndPhase()
    {
        IngameManager.Instance.EnemyPhase();
    }
}
