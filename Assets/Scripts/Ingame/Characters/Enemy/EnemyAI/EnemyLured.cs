using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;
using Ingame;

public class EnemyLured : EnemyPattern
{
    EnemyState es;
    EnemyBehaviour eb;
    public override EnemyPatternType PatternType => EnemyPatternType.Lured;
    public override List<Spot> path { get => base.path; set => base.path = value; }

    public EnemyLured(EnemyState enemyState, EnemyBehaviour enemyBehaviour)
    {
        es = enemyState;
        eb = enemyBehaviour;
    }
    public override void UpdateState()
    {
        if (eb.suspect == null)
        {
            if (eb.memoryturn <= 0)
            {
                eb.enemyPattern = new EnemyReturn(es, eb);
            }
        }
        else
        {
            if (es.suspicion[eb.suspect.GetComponent<PlayerState>().playerIndex] >= 100)
            {
                eb.memoryturn = 2;
                eb.enemyPattern = new EnemyAlert(es, eb);
                eb.AlertOthers();
            }
            else if (es.suspicion[eb.suspect.GetComponent<PlayerState>().playerIndex] >= 50)
            {
                eb.memoryturn = 2;
                eb.enemyPattern = new EnemyChase(es, eb);
                eb.AlertOthers();
            }
        }
    }
    public override void EnemyAct(Vector2Int targetPos, GameObject current)
    {
        MapManager map = IngameManager.Instance.mapManager;
        Vector2Int currentpos = map.GetGridPositionFromWorld(current.transform.position);
        map.spots[targetPos.x, targetPos.y].z = 0;

        Astar astar = new Astar(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height);
        List<Spot> p = astar.CreatePath(map.spots, map.GetGridPositionFromWorld(current.transform.position), targetPos, 1000, false);
        map.spots[targetPos.x, targetPos.y].z = 1;

        List<Spot> newPath = new List<Spot>();
        p.Reverse();
        map.spots[currentpos.x, currentpos.y].z = 0;

        if (p.Count < es.moveRange)
        {
            for (int i = 0; i < p.Count - 1; i++)
            {
                newPath.Add(p[i]);
            }
        }
        else
        {
            for (int i = 0; i < es.moveRange; i++)
            {
                newPath.Add(p[i]);
            }
        }
        path = newPath;
    }
}
