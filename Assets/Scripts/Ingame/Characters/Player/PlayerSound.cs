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

        for (int i = 0; i < enemies.Count; i++)
        {
            int x = Math.Abs(soundPos.x - map.GetGridPositionFromWorld(enemies[i].transform.position).x);
            int y = Math.Abs(soundPos.y - map.GetGridPositionFromWorld(enemies[i].transform.position).y);
            if (x + y <= soundRange)
            {
                EnemyBehaviour eb = enemies[i].GetComponent<EnemyBehaviour>();
                if (eb.enemyPattern == EnemyPattern.Guard || eb.enemyPattern == EnemyPattern.Patrol || eb.enemyPattern == EnemyPattern.Lured)
                {
                    eb.enemyPattern = EnemyPattern.Lured;
                    eb.lurePos = soundPos;
                    eb.memoryturn = 2;
                }
            }
        }
    }
}
