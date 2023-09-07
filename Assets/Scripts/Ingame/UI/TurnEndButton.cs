using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEndButton : MonoBehaviour
{
    public void EndTurn()
    {
        IngameManager.Instance.EnemyTurn();
    }
}
