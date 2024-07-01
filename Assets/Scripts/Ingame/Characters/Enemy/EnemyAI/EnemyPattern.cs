using System.Collections;
using System.Collections.Generic;
using Ingame;
using UnityEngine;

public class EnemyPattern
{
    public virtual EnemyPatternType PatternType => EnemyPatternType.Guard;
    public virtual List<Spot> path { get; set; }
    public virtual Vector2Int currentPos { get; set; }
    public virtual void UpdateState()
    {

    }
    public virtual void EnemyAct(Vector2Int targetPos, GameObject current)
    {

    }
    public virtual void Attack(GameObject current)
    {

    }
}
