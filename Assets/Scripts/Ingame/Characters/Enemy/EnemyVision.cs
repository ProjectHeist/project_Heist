using System.Collections;
using System.Collections.Generic;
using Ingame;
using UnityEngine;
using Logics;

public class EnemyVision : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Vector2Int> visionList = new List<Vector2Int>();  // 시야 타일들을 저장하는 리스트

    public void ApplyVisionToTile()
    {
        MapManager map = IngameManager.Instance.mapManager;
        EnemyBehaviour enemyBehaviour = gameObject.GetComponent<EnemyBehaviour>();
        foreach (Vector2Int vision in visionList)
        {
            GridCell gridCell = map.GetGridCellFromPosition(vision).GetComponent<GridCell>();
            gridCell.SetSight(enemyBehaviour.enemyIndex, true);
        }
    }

    public void DeleteVisionFromTile()
    {
        MapManager map = IngameManager.Instance.mapManager;
        EnemyBehaviour enemyBehaviour = gameObject.GetComponent<EnemyBehaviour>();
        foreach (Vector2Int vision in visionList)
        {
            GridCell gridCell = map.GetGridCellFromPosition(vision).GetComponent<GridCell>();
            gridCell.SetSight(enemyBehaviour.enemyIndex, false);
        }
    }

    public void visionRotate(int angle)
    {
        switch (angle)
        {
            case -90:
                for (int i = 0; i < visionList.Count; i++)
                {
                    Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
                    int x = visionList[i].x - currentPos.x;
                    int y = visionList[i].y - currentPos.y;
                    Vector2Int newPos = new Vector2Int(currentPos.x - y, currentPos.y + x);
                    visionList[i] = newPos;
                }
                break;
            case 90:
                for (int i = 0; i < visionList.Count; i++)
                {
                    Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
                    int x = visionList[i].x - currentPos.x;
                    int y = visionList[i].y - currentPos.y;
                    Vector2Int newPos = new Vector2Int(currentPos.x + y, currentPos.y - x);
                    visionList[i] = newPos;
                }
                break;
            case 180:
                for (int i = 0; i < visionList.Count; i++)
                {
                    Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
                    int x = visionList[i].x - currentPos.x;
                    int y = visionList[i].y - currentPos.y;
                    Vector2Int newPos = new Vector2Int(currentPos.x - x, currentPos.y - y);
                    visionList[i] = newPos;
                }
                break;
        }
    }

    public void visionMove(Vector2Int vector2Int)
    {
        DeleteVisionFromTile();
        for (int i = 0; i < visionList.Count; i++)
        {
            Vector2Int newPos = new Vector2Int(visionList[i].x + vector2Int.x, visionList[i].y + vector2Int.y);
            visionList[i] = newPos;
        }
        ApplyVisionToTile();
    }

    public void PlayerEnterSight(GameObject player)
    {
        EnemyBehaviour enemyBehaviour = gameObject.GetComponentInParent<EnemyBehaviour>();
        EnemyMove enemyMove = gameObject.GetComponentInParent<EnemyMove>();
        EnemyState es = gameObject.GetComponentInParent<EnemyState>();
        int idx = enemyBehaviour.enemyIndex;
        if (!IngameManager.Instance.walldetection.IsWallBetween(transform.position, player.transform.position) && !enemyBehaviour.detectedplayers.Contains(player)) // 사이에 벽이 없고, 이미 감지되지 않은 경우
        {
            enemyBehaviour.detectedplayers.Add(player);
            player.GetComponent<PlayerState>().enemyDetectedPlayer.Add(gameObject.transform.parent.gameObject);

            Vector2Int currPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position);
            PlayerState ps = player.GetComponent<PlayerState>();
            int sus = IngameManager.Instance.mapManager.GetSuspicion(currPos); //의심도 체크

            if (!es.wasDetected[ps.playerIndex])
            {
                if (sus != 0) // 금지구역에 있을 때
                {
                    es.AddSuspicion(ps.playerIndex, sus);
                }
                else if (enemyBehaviour.enemyPattern.PatternType == EnemyPatternType.Lured)
                {
                    es.AddSuspicion(ps.playerIndex, 10);
                }
            }

            if (es.suspicion[ps.playerIndex] >= 100) // 의심가는 인물이 포착될 경우
            {
                GameObject max = enemyBehaviour.GetMaxSuspicion();
                if (player == max)
                {
                    enemyBehaviour.suspect = player;
                    enemyBehaviour.memoryturn = 2;
                    if (enemyBehaviour.enemyPattern.PatternType != EnemyPatternType.Alert)
                    {
                        if (enemyMove.moving)
                            enemyMove.StopMove();
                        enemyBehaviour.enemyPattern = new EnemyAlert(es, enemyBehaviour);
                        enemyBehaviour.AlertOthers();
                    }
                }
                if (!IngameManager.Instance.spawner.policeSpawn)
                {
                    IngameManager.Instance.spawner.startspawnTimer(enemyBehaviour.suspect);
                }
            }
            else if (es.suspicion[ps.playerIndex] >= 50)
            {
                GameObject max = enemyBehaviour.GetMaxSuspicion();
                if (player == max)
                {
                    enemyBehaviour.suspect = player;
                    enemyBehaviour.memoryturn = 2;
                    if (enemyBehaviour.enemyPattern.PatternType == EnemyPatternType.Patrol || enemyBehaviour.enemyPattern.PatternType == EnemyPatternType.Guard)
                    {
                        if (enemyMove.moving)
                            enemyMove.StopMove();
                        enemyBehaviour.enemyPattern = new EnemyChase(es, enemyBehaviour);
                    }
                }
            }
        }
        else
        {
            Debug.Log("Wall is between");
        }
    }

    public void PlayerExitSight(GameObject player)
    {
        EnemyBehaviour enemyBehaviour = gameObject.GetComponentInParent<EnemyBehaviour>();
        if (enemyBehaviour.detectedplayers.Contains(player))
        {
            enemyBehaviour.detectedplayers.Remove(player);
            player.GetComponent<PlayerState>().enemyDetectedPlayer.Remove(gameObject);
        }
    }
}
