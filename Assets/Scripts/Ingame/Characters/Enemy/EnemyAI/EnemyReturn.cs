using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ingame;
using Logics;

public class EnemyReturn : EnemyPattern
{
    EnemyState es;
    EnemyBehaviour eb;
    public Vector2Int destPos;
    public override Vector2Int currentPos { get => base.currentPos; set => base.currentPos = value; }
    public override EnemyPatternType PatternType => EnemyPatternType.Return;
    public override List<Spot> path { get => base.path; set => base.path = value; }
    public EnemyReturn(EnemyState enemyState, EnemyBehaviour enemyBehaviour)
    {
        es = enemyState;
        eb = enemyBehaviour;
    }
    public override void UpdateState()
    {
        if (currentPos == destPos)
        {
            eb.enemyPattern = new EnemyPatrol(es, eb);
        }
    }
    public override void EnemyAct(Vector2Int targetPos, GameObject current)
    {
        destPos = GameManager.Instance._data.totalDB.mapDatabase.MapDataList[GameManager.Instance.mapIndex].enemyPos[eb.enemyIndex];

        MapManager map = IngameManager.Instance.mapManager;
        Astar astar = new Astar(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height);
        List<Spot> p = astar.CreatePath(map.spots, map.GetGridPositionFromWorld(current.transform.position), destPos, 1000, false);

        List<Spot> newPath = new List<Spot>();
        p.Reverse();
        if (p.Count < es.moveRange)
        {
            for (int i = 0; i < p.Count; i++)
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
