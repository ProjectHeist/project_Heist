using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;
using Ingame;

public class EnemyChase : EnemyPattern
{
    EnemyState es;
    EnemyBehaviour eb;
    public override EnemyPatternType PatternType => EnemyPatternType.Chase;
    public override List<Spot> path { get => base.path; set => base.path = value; }

    public EnemyChase(EnemyState enemyState, EnemyBehaviour enemyBehaviour)
    {
        es = enemyState;
        eb = enemyBehaviour;
    }
    public override void UpdateState()
    {
        if (eb.suspect == null) // 용의자가 없어졌을 경우
        {
            eb.memoryturn = 0;
            eb.enemyPattern = new EnemyReturn(es, eb);
        }
        else if (eb.memoryturn <= 0) // 추격하다가 시야에서 일정 시간 이상 사라졌을 경우
        {
            eb.enemyPattern = new EnemyReturn(es, eb);
            es.isSuspect[eb.suspect.GetComponent<PlayerState>().playerIndex] = false;
            eb.suspect = null;
        }
        else if (eb.detectedplayers.Contains(eb.suspect)) // 시야 내에 용의자가 있는 경우
        {
            if (es.suspicion[eb.suspect.GetComponent<PlayerState>().playerIndex] >= 100)
            {
                eb.memoryturn = 2;
                eb.enemyPattern = new EnemyAlert(es, eb);
                eb.AlertOthers();
            }
            else
            {
                eb.memoryturn = 2;
                eb.AlertOthers();
            }
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
        List<Spot> p = astar.CreatePath(map.spots, map.GetGridPositionFromWorld(current.transform.position), targetPos, 1000, false);
        map.spots[targetPos.x, targetPos.y].z = 1;

        List<Spot> newPath = new List<Spot>();
        p.Reverse();
        map.spots[currentpos.x, currentpos.y].z = 0;

        if (path.Count < es.moveRange)
        {
            for (int i = 0; i < path.Count - 4; i++)
            {
                newPath.Add(path[i]);
            }
        }
        else
        {
            for (int i = 0; i < es.moveRange - 3; i++)
            {
                newPath.Add(path[i]);
            }
        }
        path = newPath;
    }
}
