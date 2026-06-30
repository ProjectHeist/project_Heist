using System.Collections;
using System.Collections.Generic;
using Ingame;
using UnityEngine;
using Logics;

public class EnemyPattern
{
    protected EnemyState es;
    public virtual EnemyPatternType PatternType => EnemyPatternType.Guard;
    public virtual List<Spot> path { get; set; }
    public virtual Vector2Int currentPos { get; set; }
    public virtual void UpdateState()
    {

    }
    public virtual void SetPath(Vector2Int targetPos, GameObject current)
    {
        MapManager map = IngameManager.Instance.mapManager;
        Vector2Int currentpos = map.GetGridPositionFromWorld(current.transform.position);
        map.spots[targetPos.x, targetPos.y].z = 0;

        Astar astar = new Astar(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height);
        List<Spot> p = astar.CreatePath(map.spots, map.GetGridPositionFromWorld(current.transform.position), targetPos, 10000, false);
        map.spots[targetPos.x, targetPos.y].z = 3;
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
    }
    public virtual void EnemyAct(Vector2Int targetPos, GameObject current)
    {

    }
    public virtual void Attack(GameObject current)
    {

    }
}
