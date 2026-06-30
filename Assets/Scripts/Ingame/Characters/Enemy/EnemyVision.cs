using System.Collections;
using System.Collections.Generic;
using Ingame;
using UnityEngine;
using Logics;
using System.Numerics;

public class EnemyVision : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Vector2Int> originalVisionList = new List<Vector2Int>();  // 시야 타일들을 저장하는 리스트
    public List<Vector2Int> visionList = new List<Vector2Int>();  // 시야 가려짐 현상을 적용한 후의 시야 타일 리스트 

    public void ApplyVisionToTile()
    {
        visionList.Clear();
        visionList.AddRange(originalVisionList);
        MapManager map = IngameManager.Instance.mapManager;
        EnemyBehaviour enemyBehaviour = gameObject.GetComponent<EnemyBehaviour>();
        deleteVisionUnavailable();
        foreach (Vector2Int vision in visionList)
        {
            GridCell gridCell = map.GetGridCellFromPosition(vision).GetComponent<GridCell>();
            gridCell.SetSight(enemyBehaviour.enemyIndex, true);
        }
    }

    private void deleteVisionUnavailable()
    {
        MapManager map = IngameManager.Instance.mapManager;
        Vector2Int currentPos = map.GetGridPositionFromWorld(gameObject.transform.position);
        List<Vector2Int> newVisionList = new List<Vector2Int>();
        foreach (Vector2Int vision in visionList) //범위 밖 타일 제거
        {
            if (!(vision.x < 0 || vision.x >= map.width || vision.y < 0 || vision.y >= map.height))
            {
                bool notblocked = IngameManager.Instance.walldetection.HasLineOfSight(currentPos, vision);
                Debug.Log(notblocked);
                if (notblocked)
                {
                    newVisionList.Add(vision);
                }
                else
                {
                    GridCell gridCell = map.GetGridCellFromPosition(vision).GetComponent<GridCell>();
                    gridCell.SetSight(gameObject.GetComponent<EnemyBehaviour>().enemyIndex, false);
                }
            }
        }
        visionList.Clear();
        visionList.AddRange(newVisionList);
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
            case 90:
                for (int i = 0; i < originalVisionList.Count; i++)
                {
                    Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
                    int dx = originalVisionList[i].x - currentPos.x;
                    int dy = originalVisionList[i].y - currentPos.y;
                    Vector2Int newPos = new Vector2Int(currentPos.x + dy, currentPos.y - dx);
                    originalVisionList[i] = newPos;
                }
                break;
            case -90:
                for (int i = 0; i < originalVisionList.Count; i++)
                {
                    Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
                    int dx = originalVisionList[i].x - currentPos.x;
                    int dy = originalVisionList[i].y - currentPos.y;
                    Vector2Int newPos = new Vector2Int(currentPos.x - dy, currentPos.y + dx);
                    originalVisionList[i] = newPos;
                }
                break;
            case 180:
                for (int i = 0; i < originalVisionList.Count; i++)
                {
                    Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
                    int x = 2 * currentPos.x - originalVisionList[i].x;
                    int y = 2 * currentPos.y - originalVisionList[i].y;
                    Vector2Int newPos = new Vector2Int(x, y);
                    originalVisionList[i] = newPos;
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
        for (int i = 0; i < originalVisionList.Count; i++)
        {
            Vector2Int newPos = new Vector2Int(originalVisionList[i].x + vector2Int.x, originalVisionList[i].y + vector2Int.y);
            originalVisionList[i] = newPos;
        }
        ApplyVisionToTile();
    }

    public void PlayerEnterSight(GameObject player)
    {
        EnemyBehaviour enemyBehaviour = gameObject.GetComponentInParent<EnemyBehaviour>();
        EnemyMove enemyMove = gameObject.GetComponentInParent<EnemyMove>();
        EnemyState es = gameObject.GetComponentInParent<EnemyState>();
        int idx = enemyBehaviour.enemyIndex;
        Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(transform.position);
        Vector2Int playerPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position);
        if (IngameManager.Instance.walldetection.HasLineOfSight(currentPos, playerPos) && !enemyBehaviour.detectedplayers.Contains(player)) // 사이에 벽이 없고, 이미 감지되지 않은 경우
        {
            enemyBehaviour.detectedplayers.Add(player);
            Debug.Log("parent is:" + gameObject);
            player.GetComponent<PlayerState>().enemyDetectedPlayer.Add(gameObject);

            Vector2Int currPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position);
            PlayerState ps = player.GetComponent<PlayerState>();
            int sus = IngameManager.Instance.mapManager.GetSuspicion(currPos); //의심도 체크

            if (!es.wasDetected[ps.playerIndex])
            {
                if (sus != 0) // 금지구역에 있을 때
                {
                    es.IncreaseSuspicion(playerPos, currentPos, ps.playerIndex);
                }
                else if (enemyBehaviour.enemyPattern.PatternType == EnemyPatternType.Lured)
                {
                    es.IncreaseSuspicion(playerPos, currentPos, ps.playerIndex);
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
