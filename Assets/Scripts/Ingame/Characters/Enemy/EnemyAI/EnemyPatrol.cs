using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;
using Ingame;

public class EnemyPatrol : EnemyPattern
{
    int currentRoute = 0;
    EnemyState es;
    EnemyBehaviour eb;
    public override EnemyPatternType PatternType => EnemyPatternType.Patrol;
    public override List<Spot> path { get => base.path; set => base.path = value; }
    public EnemyPatrol(EnemyState enemyState, EnemyBehaviour enemyBehaviour)
    {
        es = enemyState;
        eb = enemyBehaviour;
    }
    public override void UpdateState()
    {
        if (eb.suspect != null)
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
        if (es.routeNum != -1)
        {
            MapManager map = IngameManager.Instance.mapManager;
            Vector2Int currentpos = map.GetGridPositionFromWorld(current.transform.position);

            List<List<Spot>> routes = map.getPatrolRoutes(map.enemyPos[eb.enemyIndex], es.routeNum);
            List<Spot> p = routes[currentRoute];

            Vector2Int targetpos = new Vector2Int(p[p.Count - 1].X, p[p.Count - 1].Y);
            map.spots[currentpos.x, currentpos.y].z = 0;
            map.spots[targetpos.x, targetpos.y].z = 1;

            if (currentRoute == routes.Count - 1)
            {
                currentRoute = 0;
            }
            else
            {
                currentRoute++;
            }
            path = p;
        }
    }
}
