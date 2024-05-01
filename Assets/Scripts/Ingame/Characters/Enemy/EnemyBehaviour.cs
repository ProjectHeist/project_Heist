using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Logics;
using JetBrains.Annotations;
using Unity.VisualScripting;

public enum EnemyPattern
{
    Guard,
    Patrol,
    Chase,
    Alert,
    Lured
}

namespace Ingame
{
    public class EnemyBehaviour : MonoBehaviour
    {
        public EnemyPattern enemyPattern = EnemyPattern.Patrol;
        public int currentRoute = 0;

        private EnemyState es;
        private int currentPathIndex;
        private float moveSpeed = 5.0f;
        public bool patrolling = false;
        public int enemyIndex;
        Coroutine patrol;

        public GameObject suspect; // 시야 내에 들어온 용의자
        public List<GameObject> detectedplayers = new List<GameObject>();
        public int memoryturn = 0; // 적의 기억력. 해당 턴 동안 적은 용의자를 지속해서 추격한다
        public Vector2Int lurePos;

        public void CheckAndChangeState()
        {
            if (suspect != null && !suspect.activeSelf) // 중간에 죽었을 경우
            {
                enemyPattern = EnemyPattern.Guard;
                detectedplayers.Remove(suspect);
                suspect = null;
            }
            else if ((enemyPattern == EnemyPattern.Chase || enemyPattern == EnemyPattern.Alert) && memoryturn <= 0) // 추격 상태에서 오랫동안 시야에 보이지 않았을 경우
            {
                enemyPattern = EnemyPattern.Guard;
                suspect.GetComponent<PlayerState>().isSuspect[enemyIndex] = false;
                suspect = null;
            }
            else if (enemyPattern == EnemyPattern.Lured && memoryturn <= 0) // 어그로가 끌렸지만 보이지 않을 때
            {
                enemyPattern = EnemyPattern.Guard;
            }
            else // 그 이외의 경우
            {
                if (detectedplayers.Count > 0) //감지해서 금지구역에 있는지 확인, 있다면 의심도 증가
                {
                    for (int i = 0; i < detectedplayers.Count; i++)
                    {
                        Vector2Int currPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(detectedplayers[i].transform.position);
                        int sus = IngameManager.Instance.mapManager.GetSuspicion(currPos); //의심도 체크
                        if (sus != 0) // 금지구역에 있을 때
                        {
                            detectedplayers[i].GetComponent<PlayerState>().suspicion[enemyIndex] += sus;
                        }
                        else if (enemyPattern == EnemyPattern.Lured) // 어그로가 끌린 상태에서 플레이어를 발견했을 때
                        {
                            detectedplayers[i].GetComponent<PlayerState>().suspicion[enemyIndex] += 50;
                        }
                    }
                    GameObject max = GetMaxSuspicion();
                    if (max.GetComponent<PlayerState>().suspicion[enemyIndex] >= 50)
                    {
                        suspect = max;
                    }
                    if (suspect != null)
                    {
                        if (suspect.GetComponent<PlayerState>().suspicion[enemyIndex] >= 100)
                        {
                            memoryturn = 2;
                            enemyPattern = EnemyPattern.Alert;
                            AlertOthers();
                        }
                        else if (suspect.GetComponent<PlayerState>().suspicion[enemyIndex] >= 50)
                        {
                            memoryturn = 2;
                            if (enemyPattern == EnemyPattern.Patrol || enemyPattern == EnemyPattern.Guard)
                            {
                                enemyPattern = EnemyPattern.Chase;
                            }
                            AlertOthers();
                        }
                    }
                }
            }
        }

        public void AlertOthers()
        {
            List<GameObject> enemies = IngameManager.Instance.enemies;
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyBehaviour eb = enemies[i].GetComponent<EnemyBehaviour>();
                Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
                Vector2Int targetPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(enemies[i].transform.position);
                if (InRange(currentPos, targetPos, gameObject.GetComponent<EnemyState>().alertRange))
                {
                    eb.suspect = suspect;
                    int currentsuspicion = suspect.GetComponent<PlayerState>().suspicion[enemyIndex];
                    if (currentsuspicion >= 100) // Alert
                    {
                        enemies[i].GetComponent<EnemyBehaviour>().enemyPattern = EnemyPattern.Alert;
                        suspect.GetComponent<PlayerState>().SetSuspicion(eb.enemyIndex, currentsuspicion);
                    }
                    else if (currentsuspicion >= 50) // Chase
                    {
                        enemies[i].GetComponent<EnemyBehaviour>().enemyPattern = EnemyPattern.Chase;
                        suspect.GetComponent<PlayerState>().SetSuspicion(eb.enemyIndex, currentsuspicion);
                    }
                }
            }
        }

        public void memoryCount()
        {
            if (detectedplayers.Contains(suspect)) // 만약 용의자가 시야에 있을 경우
            {
                memoryturn = 2;
            }
            else
            {
                memoryturn--;
            }
        }
        public void EnemyAct()
        {
            es = gameObject.GetComponent<EnemyState>();
            CheckAndChangeState();
            memoryCount();
            Debug.Log(enemyPattern);
            MapManager map = IngameManager.Instance.mapManager;
            switch (enemyPattern)
            {
                case EnemyPattern.Guard:
                    break;
                case EnemyPattern.Patrol:
                    Patrol();
                    break;
                case EnemyPattern.Chase:
                    Chase(map.GetGridPositionFromWorld(suspect.transform.position));
                    changeFaceDir(map.GetGridPositionFromWorld(suspect.transform.position));
                    Debug.Log("Chase");
                    break;
                case EnemyPattern.Alert:
                    Chase(map.GetGridPositionFromWorld(suspect.transform.position));
                    changeFaceDir(map.GetGridPositionFromWorld(suspect.transform.position));
                    Debug.Log("Alert");
                    break;
                case EnemyPattern.Lured:
                    Chase(lurePos);
                    changeFaceDir(lurePos);
                    Debug.Log("Lured");
                    break;
            }
            Debug.Log(enemyPattern);
        }

        public void Patrol()
        {
            if (es.routeNum != -1)
            {
                PatrolRoute route = GameManager.Instance._data.totalDB.mapDatabase.MapDataList[GameManager.Instance.mapIndex].patrolRoutes[es.routeNum]; // 루트 가져오고
                var currRoute = route.routes[currentRoute];

                List<Vector2Int> path = new List<Vector2Int>();
                Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(transform.position);
                for (int i = 0; i < currRoute.direction.Count; i++)
                {
                    Vector2Int moveTo = GetModifiedDist(currRoute.direction[i], currRoute.movedist[i]);
                    path.Add(currentPos + moveTo);
                    currentPos += moveTo;
                }
                patrol = StartCoroutine(Patrolling(path));
                if (currentRoute == route.routes.Count - 1)
                {
                    currentRoute = 0;
                }
                else
                {
                    currentRoute++;
                }
            }
        }

        public Vector2Int GetModifiedDist(string rot, int dist)
        {
            switch (rot)
            {
                case "+x": //+x
                    return new Vector2Int(dist, 0);
                case "-x": //-x
                    return new Vector2Int(-dist, 0);
                case "+y": //+y
                    return new Vector2Int(0, dist);
                case "-y": //-y
                    return new Vector2Int(0, -dist);
            }
            return new Vector2Int(0, 0);
        }

        IEnumerator Patrolling(List<Vector2Int> path)
        {
            bool arrived = false;
            int currentPathIndex = 0;
            patrolling = true;
            while (!arrived)
            {
                Vector2Int targetPos = path[currentPathIndex];
                Vector3 targetPosition = IngameManager.Instance.mapManager.GetWorldPositionFromGridPosition(targetPos);
                if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    Vector3 moveDir = (targetPosition - transform.position).normalized;
                    transform.position = transform.position + moveDir * moveSpeed * Time.deltaTime;
                }
                else
                {
                    currentPathIndex++;
                    if (currentPathIndex >= path.Count)
                    {
                        patrolling = false;
                        break;
                    }
                }
                yield return null;
            }
        }

        /*public void Detect()
        {
            suspect = null;
            MapManager map = IngameManager.Instance.mapManager;
            List<GameObject> players = IngameManager.Instance.players;
            for (int i = 0; i < players.Count; i++)
            {
                if (InRange(map.GetGridPositionFromWorld(gameObject.transform.position), map.GetGridPositionFromWorld(players[i].transform.position), es.detectRange))
                {
                    suspect = players[i];
                    enemyPattern = EnemyPattern.Chase;
                    if (InRange(map.GetGridPositionFromWorld(gameObject.transform.position), map.GetGridPositionFromWorld(players[i].transform.position), es.maxAttackRange))
                    {
                        enemyPattern = EnemyPattern.Attack;
                        changeFaceDir();
                        break;
                    }
                    changeFaceDir();
                }
            }
            if (suspect == null && es.routeNum == -1)
            {
                enemyPattern = EnemyPattern.Guard;
            }
            else if (suspect == null)
            {
                enemyPattern = EnemyPattern.Patrol;
            }
        }*/

        public bool InRange(Vector2Int startPos, Vector2Int targetPos, int range)
        {
            EnemyState es = gameObject.GetComponent<EnemyState>();
            int xdist = targetPos.x - startPos.x;
            int ydist = targetPos.y - startPos.y;
            int dist = Math.Abs(xdist) + Math.Abs(ydist);
            if (dist < range)
            {
                return true;
            }
            return false;
        }

        public void changeFaceDir(Vector2Int targetPos)
        {
            Vector2Int startPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
            int xdist = targetPos.x - startPos.x;
            int ydist = targetPos.y - startPos.y;
            if (Math.Abs(xdist) > Math.Abs(ydist))
            {
                if (xdist > 0)
                {
                    int prev = es.faceDir;
                    es.faceDir = 0;
                    Rotate(prev, es.faceDir);
                }
                else
                {
                    int prev = es.faceDir;
                    es.faceDir = 2;
                    Rotate(prev, es.faceDir);
                }
            }
            else if (Math.Abs(ydist) > Math.Abs(xdist))
            {
                if (ydist > 0)
                {
                    int prev = es.faceDir;
                    es.faceDir = 1;
                    Rotate(prev, es.faceDir);
                }
                else
                {
                    int prev = es.faceDir;
                    es.faceDir = 3;
                    Rotate(prev, es.faceDir);
                }
            }
        }

        private void Rotate(int prev, int curr)
        {
            Vector3 angle = new Vector3(0, 90, 0) * (prev - curr);
            transform.eulerAngles = angle;
        }

        public void Chase(Vector2Int targetPos)
        {
            MapManager map = IngameManager.Instance.mapManager;
            Vector2Int currentpos = map.GetGridPositionFromWorld(gameObject.transform.position);
            map.spots[targetPos.x, targetPos.y].z = 0;
            Astar astar = new Astar(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height);
            List<Spot> path = astar.CreatePath(map.spots, map.GetGridPositionFromWorld(gameObject.transform.position), targetPos, 1000);
            map.spots[targetPos.x, targetPos.y].z = 1;
            List<Spot> newPath = new List<Spot>();
            path.Reverse();
            map.spots[currentpos.x, currentpos.y].z = 0;
            if (path.Count < es.moveRange)
            {
                for (int i = 0; i < path.Count - 4; i++)
                {
                    newPath.Add(path[i]);
                }
            }
            else
            {
                for (int i = 0; i < es.moveRange - 3; i++)
                {
                    newPath.Add(path[i]);
                }
            }
            StartCoroutine(Move(newPath));
        }

        IEnumerator Move(List<Spot> path)
        {
            MapManager map = IngameManager.Instance.mapManager;
            currentPathIndex = 0;
            while (currentPathIndex < path.Count)
            {
                Vector2Int target = path[currentPathIndex].position;
                Vector3 targetPosition = IngameManager.Instance.mapManager.GetWorldPositionFromGridPosition(target);
                if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    Vector3 moveDir = (targetPosition - transform.position).normalized;
                    float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                    transform.position = transform.position + moveDir * moveSpeed * Time.deltaTime;
                }
                else
                {
                    currentPathIndex++;
                }
                yield return null;
            }
            Vector2Int currentpos = map.GetGridPositionFromWorld(gameObject.transform.position);
            map.spots[currentpos.x, currentpos.y].z = 1;

            if (suspect != null)
            {
                if (InRange(map.GetGridPositionFromWorld(gameObject.transform.position), map.GetGridPositionFromWorld(suspect.transform.position), es.maxAttackRange) && enemyPattern == EnemyPattern.Alert)
                {
                    Attack();
                }
            }
        }

        public void Attack()
        {
            suspect.GetComponent<PlayerState>().OnPlayerHit(es.damage);
            Debug.Log("Attack");
        }

        public void StopPatrol() //즉시 행동을 멈추는 함수. 상태 전환에 주로 사용
        {
            StopCoroutine(patrol);
            patrolling = false;
        }

        public GameObject GetMaxSuspicion()
        {
            int maxVal = -1;
            int maxIdx = -1;
            for (int i = 0; i < detectedplayers.Count; i++)
            {
                int sus = detectedplayers[i].GetComponent<PlayerState>().suspicion[enemyIndex];
                if (sus > maxVal)
                {
                    maxVal = sus;
                    maxIdx = i;
                }
            }
            if (maxIdx != -1)
            {
                return detectedplayers[maxIdx];
            }
            else
            {
                return null;
            }
        }
    }
}