using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;
using System;
using Ingame;

public class PlayerSound : MonoBehaviour
{
    public void Notify(Vector2Int soundPos, int soundRange)
    {
        List<GameObject> enemies = IngameManager.Instance.enemies;
        MapManager map = IngameManager.Instance.mapManager;
        PlayerState ps = gameObject.GetComponent<PlayerState>();

        if (ps.enemyDetectedPlayer.Count > 0)
        {
            for (int i = 0; i < ps.enemyDetectedPlayer.Count; i++)
            {
                EnemyBehaviour eb = ps.enemyDetectedPlayer[i].GetComponent<EnemyBehaviour>();
                EnemyState es = ps.enemyDetectedPlayer[i].GetComponent<EnemyState>();
                eb.enemyPattern = EnemyPatternType.Alert;
                eb.suspect = gameObject;
                eb.memoryturn = 2;

                es.suspicion[ps.playerIndex] = 100;
                es.isSuspect[ps.playerIndex] = true;
            }
        }
        else
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                int x = Math.Abs(soundPos.x - map.GetGridPositionFromWorld(enemies[i].transform.position).x);
                int y = Math.Abs(soundPos.y - map.GetGridPositionFromWorld(enemies[i].transform.position).y);
                if (x + y <= soundRange)
                {
                    EnemyBehaviour eb = enemies[i].GetComponent<EnemyBehaviour>();
                    if (eb.enemyPattern == EnemyPatternType.Guard || eb.enemyPattern == EnemyPatternType.Patrol || eb.enemyPattern == EnemyPatternType.Lured)
                    {
                        eb.enemyPattern = EnemyPatternType.Lured;
                        eb.lurePos = soundPos;
                        eb.memoryturn = 2;
                    }
                }
            }
        }
    }
}
