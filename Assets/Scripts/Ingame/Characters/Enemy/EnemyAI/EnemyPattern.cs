using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPattern
{
    public virtual EnemyPatternType PatternType => EnemyPatternType.Guard;
    protected virtual void UpdateState()
    {

    }
    protected virtual void EnemyAct()
    {

    }
}
