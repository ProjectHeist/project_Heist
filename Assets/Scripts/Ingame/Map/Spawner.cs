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
                e.GetComponent<EnemyBehaviour>().enemyPattern = EnemyPatternType.Alert;
                e.GetComponent<EnemyBehaviour>().memoryturn = 3;
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
        mapManager.spots[startPos.x, startPos.y].z = 1;
        GameObject enemyobj = Object.Instantiate(enemy, initPos, Quaternion.identity) as GameObject;
        enemyobj.GetComponent<EnemyState>().GetEnemyInfo();
        enemyobj.GetComponent<EnemyState>().suspicionInit();
        enemyobj.GetComponent<EnemyBehaviour>().enemyIndex = index;
        enemyobj.GetComponent<EnemyState>().routeNum = routeNum;
        HPBar hp = enemyobj.GetComponentInChildren<HPBar>();
        hp.SetMaxHealth(enemyobj.GetComponent<EnemyState>().HP);
        CreateEnemyVision(enemyobj.GetComponent<EnemyState>().detectRange, enemyobj.transform.GetChild(2).gameObject);
        IngameManager.Instance.enemies.Add(enemyobj);
        return enemyobj;
    }

    private void CreateEnemyVision(int range, GameObject vision)
    {
        int facedir = vision.GetComponentInParent<EnemyState>().faceDir;
        GameObject enemyModel = vision.transform.parent.gameObject.GetComponent<EnemyBehaviour>().enemyModel;
        switch (facedir)
        {
            case 0:
                enemyModel.transform.eulerAngles = new Vector3(0, 90, 0);
                for (int i = 0; i < range; i++) //세로
                {
                    for (int j = -i; j < i + 1; j++) //가로
                    {
                        BoxCollider box = vision.AddComponent<BoxCollider>();
                        box.center = new Vector3(range - i, 0, j);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < range; i++) //세로
                {
                    for (int j = -i; j < i + 1; j++) //가로
                    {
                        BoxCollider box = vision.AddComponent<BoxCollider>();
                        box.center = new Vector3(j, 0, range - i);
                    }
                }
                break;
            case 2:
                enemyModel.transform.eulerAngles = new Vector3(0, -90, 0);
                for (int i = 0; i < range; i++) //세로
                {
                    for (int j = -i; j < i + 1; j++) //가로
                    {
                        BoxCollider box = vision.AddComponent<BoxCollider>();
                        box.center = new Vector3(-(range - i), 0, j);
                    }
                }
                break;
            case 3:
                enemyModel.transform.eulerAngles = new Vector3(0, 180, 0);
                for (int i = 0; i < range; i++) //세로
                {
                    for (int j = -i; j < i + 1; j++) //가로
                    {
                        BoxCollider box = vision.AddComponent<BoxCollider>();
                        box.center = new Vector3(-(range - i), 0, j);
                    }
                }
                break;
        }
    }

}
