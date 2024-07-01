using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;
using Ingame;
using Unity.VisualScripting;

public class EnemyAlert : EnemyPattern
{
    EnemyState es;
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
        MapManager map = IngameManager.Instance.mapManager;
        Vector2Int currentpos = map.GetGridPositionFromWorld(current.transform.position);
        map.spots[targetPos.x, targetPos.y].z = 0;

        Astar astar = new Astar(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height);
        List<Spot> p = astar.CreatePath(map.spots, map.GetGridPositionFromWorld(current.transform.position), targetPos, 10000, false);
        map.spots[targetPos.x, targetPos.y].z = 1;
        List<Spot> newPath = new List<Spot>();

        p.Reverse();
        map.spots[currentpos.x, currentpos.y].z = 0;
        if (p.Count < es.moveRange)
        {
            for (int i = 0; i < p.Count - 4; i++)
            {
                newPath.Add(p[i]);
            }
        }
        else
        {
            for (int i = 0; i < es.moveRange - 3; i++)
            {
                newPath.Add(p[i]);
            }
        }
        path = newPath;
        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log("path: " + path[i].X + " " + path[i].Y);
        }
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
