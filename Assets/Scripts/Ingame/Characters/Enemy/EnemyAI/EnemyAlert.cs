using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;
using Ingame;
using Unity.VisualScripting;

public class EnemyAlert : EnemyPattern
{
    EnemyBehaviour eb;
    public override EnemyPatternType PatternType => EnemyPatternType.Alert;
    public override List<Spot> path { get => base.path; set => base.path = value; }

    public EnemyAlert(EnemyState enemyState, EnemyBehaviour enemyBehaviour)
    {
        es = enemyState;
        eb = enemyBehaviour;
    }
    public override void UpdateState()
    {
        if (eb.suspect == null)
        {
            eb.memoryturn = 0;
            eb.enemyPattern = new EnemyReturn(es, eb);
        }
        else if (eb.memoryturn <= 0)
        {
            eb.enemyPattern = new EnemyReturn(es, eb);
            es.isSuspect[eb.suspect.GetComponent<PlayerState>().playerIndex] = false;
            eb.suspect = null;
        }
        else if (eb.detectedplayers.Contains(eb.suspect))
        {
            eb.memoryturn = 2;
            eb.AlertOthers();
        }
        else
        {
            eb.AlertOthers();
        }
    }
    public override void EnemyAct(Vector2Int targetPos, GameObject current)
    {

    }

    public override void Attack(GameObject current)
    {
        MapManager map = IngameManager.Instance.mapManager;
        if (eb.suspect != null)
        {
            bool InRange = current.GetComponent<EnemyMovement>().InRange(map.GetGridPositionFromWorld(current.transform.position), map.GetGridPositionFromWorld(eb.suspect.transform.position), es.maxAttackRange);
            if (InRange)
            {
                eb.suspect.GetComponent<PlayerState>().OnPlayerHit(es.damage); //Attack
                Debug.Log("Attack");
            }
        }
    }
}
