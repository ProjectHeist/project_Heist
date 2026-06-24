using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ingame;
using Logics;

public class Spawner
{
    public bool policeSpawn = false;
    public int spawnTime = -1;
    private GameObject enemy;
    private GameObject suspect;
    private MapManager mapManager;
    public Spawner(MapManager map, GameObject prefab)
    {
        mapManager = map;
        enemy = prefab;
    }

    public void startspawnTimer(GameObject sus)
    {
        policeSpawn = true;
        spawnTime = mapManager.spawnTime;
        suspect = sus;
    }

    public void stopSpawnTimer()
    {
        policeSpawn = false;
        suspect = null;
        spawnTime = -1;
    }

    public void spawnPolice()
    {
        if (policeSpawn && spawnTime == 0)
        {
            for (int i = 0; i < mapManager.spawnPos.Length; i++)
            {
                GameObject e = CreateEnemy(IngameManager.Instance.enemies.Count, 0, mapManager.spawnPos[i]);
                e.GetComponent<EnemyState>().suspicion[suspect.GetComponent<PlayerState>().playerIndex] = 100;
                e.GetComponent<EnemyBehaviour>().suspect = suspect;
                e.GetComponent<EnemyBehaviour>().memoryturn = 3;
                e.GetComponent<EnemyBehaviour>().init(EnemyPatternType.Alert);
            }
            spawnTime = mapManager.spawnTime;
        }
        else if (spawnTime > 0)
        {
            spawnTime--;
        }
    }

    public GameObject CreateEnemy(int index, int routeNum, Vector2Int startPos)
    {
        Vector3 initPos = mapManager.GetWorldPositionFromGridPosition(startPos);
        mapManager.spots[startPos.x, startPos.y].z = 3;
        GameObject enemyobj = Object.Instantiate(enemy, initPos, Quaternion.identity) as GameObject;
        enemyobj.GetComponent<EnemyState>().GetEnemyInfo();
        enemyobj.GetComponent<EnemyState>().suspicionInit();
        enemyobj.GetComponent<EnemyBehaviour>().enemyIndex = index;
        enemyobj.GetComponent<EnemyState>().routeNum = routeNum;
        HPBar hp = enemyobj.GetComponentInChildren<HPBar>();
        hp.SetMaxHealth(enemyobj.GetComponent<EnemyState>().HP);
        CreateEnemyVision(enemyobj.GetComponent<EnemyState>().detectRange, enemyobj);
        enemyobj.GetComponent<EnemyBehaviour>().init(EnemyPatternType.Patrol);
        IngameManager.Instance.enemies.Add(enemyobj);
        mapManager.GetGridCellFromPosition(startPos).GetComponent<GridCell>().SetEnemy(index, true);
        return enemyobj;
    }

    private void CreateEnemyVision(int range, GameObject enemyobj)
    {
        int facedir = enemyobj.GetComponent<EnemyState>().faceDir;
        Vector2 faceVector = new Vector2();
        GameObject enemyModel = enemyobj.GetComponent<EnemyBehaviour>().enemyModel;
        EnemyVision enemyVision = enemyobj.GetComponent<EnemyVision>();
        Vector2Int enemyPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(enemyobj.transform.position);

        switch (facedir)
        {
            case 0:
                enemyModel.transform.eulerAngles = new Vector3(0, 90, 0);
                faceVector = new Vector2(1, 0);
                /*for (int i = 0; i < range; i++) //세로
                {
                    for (int j = -i; j < i + 1; j++) //가로
                    {
                        Vector2Int visionComp = new Vector2Int(enemyPos.x + range - i, enemyPos.y + j);
                        enemyVision.originalVisionList.Add(visionComp);
                    }
                }*/
                break;
            case 1:
                faceVector = new Vector2(0, 1);
                /*for (int i = 0; i < range; i++) //세로
                {
                    for (int j = -i; j < i + 1; j++) //가로
                    {
                        Vector2Int visionComp = new Vector2Int(enemyPos.x + j, enemyPos.y + range - i);
                        enemyVision.originalVisionList.Add(visionComp);
                    }
                }*/
                break;
            case 2:
                faceVector = new Vector2(-1, 0);
                enemyModel.transform.eulerAngles = new Vector3(0, -90, 0);
                /*for (int i = 0; i < range; i++) //세로
                {
                    for (int j = -i; j < i + 1; j++) //가로
                    {
                        Vector2Int visionComp = new Vector2Int(enemyPos.x - (range - i), enemyPos.y + j);
                        enemyVision.originalVisionList.Add(visionComp);
                    }
                }*/
                break;
            case 3:
                faceVector = new Vector2(0, -1);
                enemyModel.transform.eulerAngles = new Vector3(0, 180, 0);
                /*for (int i = 0; i < range; i++) //세로
                {
                    for (int j = -i; j < i + 1; j++) //가로
                    {
                        Vector2Int visionComp = new Vector2Int(enemyPos.x - (range - i), enemyPos.y + j);
                        enemyVision.originalVisionList.Add(visionComp);
                    }
                }*/
                break;
        }

        const float HalfAngle = 60f;
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                if (!(x == 0 && y == 0) && range >= Mathf.Sqrt(x * x + y * y))
                {
                    Vector2Int visionComp = new Vector2Int(enemyPos.x + x, enemyPos.y + y);
                    Vector2 dirToComp = new Vector2(x, y).normalized;
                    float dot = Vector2.Dot(dirToComp, faceVector);
                    if (Mathf.Acos(Mathf.Clamp(dot, -1f, 1f)) * Mathf.Rad2Deg <= HalfAngle)
                    {
                        enemyVision.originalVisionList.Add(visionComp);
                    }
                }
            }
        }
        enemyVision.ApplyVisionToTile();
    }


}
