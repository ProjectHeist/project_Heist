using System.Collections;
using System.Collections.Generic;
using Logics;
using UnityEngine;
using Ingame;

enum InvestigateCause
{
    gunShot,
    WitnessHit,
    SelfHit
}

public class EnemyInvestigate : EnemyPattern
{
    EnemyBehaviour eb;
    public EnemyInvestigate(EnemyState enemyState, EnemyBehaviour enemyBehaviour)
    {
        es = enemyState;
        eb = enemyBehaviour;
    }
    public override void UpdateState()
    {

    }
}
