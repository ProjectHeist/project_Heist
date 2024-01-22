using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Logics;
using JetBrains.Annotations;

enum EnemyPattern
{
    Guard,
    Patrol,
    Chase,
    Attack
}

namespace Ingame
{
    public class EnemyBehaviour : MonoBehaviour
    {
        EnemyPattern enemyPattern = EnemyPattern.Patrol;
        public int currentRoute = 0;
        public GameObject target; // 목표로 정한 플레이어
        private EnemyState es;
        private int currentPathIndex;
        private float moveSpeed = 30.0f;

        public void EnemyAct()
        {
            es = gameObject.GetComponent<EnemyState>();
            Detect();
            switch (enemyPattern)
            {
                case EnemyPattern.Guard:
                    break;
                case EnemyPattern.Patrol:
                    Patrol();
                    break;
                case EnemyPattern.Chase:
                    Chase();
                    changeFaceDir();
                    break;
                case EnemyPattern.Attack:
                    Attack();
                    changeFaceDir();
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
                for (int i = 0; i < currRoute.movedist.Count; i++) //현재 루트에 있는 이동 명령을 실행
                {
                    Vector2Int target = IngameManager.Instance.mapManager.GetGridPositionFromWorld(transform.position) + GetModifiedDist(currRoute.direction[i], currRoute.movedist[i]);
                    StartCoroutine(Move(target));
                }
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

        IEnumerator Move(Vector2Int dest)
        {
            bool arrived = false;
            while (!arrived)
            {
                Vector3 targetPosition = IngameManager.Instance.mapManager.GetWorldPositionFromGridPosition(dest);
                if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    Debug.Log("엄준식");
                    Vector3 moveDir = (targetPosition - transform.position).normalized;
                    transform.position = transform.position + moveDir * moveSpeed * Time.deltaTime;
                }
                else
                {
                    arrived = true;
                }
                yield return null;
            }
        }

        public void Detect()
        {
            target = null;
            MapManager map = IngameManager.Instance.mapManager;
            List<GameObject> players = IngameManager.Instance.players;
            for (int i = 0; i < players.Count; i++)
            {
                if (InRange(map.GetGridPositionFromWorld(gameObject.transform.position), map.GetGridPositionFromWorld(players[i].transform.position), es.detectRange))
                {
                    target = players[i];
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
            if (target == null && es.routeNum == -1)
            {
                enemyPattern = EnemyPattern.Guard;
            }
            else if (target == null)
            {
                enemyPattern = EnemyPattern.Patrol;
            }
        }

        public bool InRange(Vector2Int startPos, Vector2Int targetPos, int range)
        {
            EnemyState es = gameObject.GetComponent<EnemyState>();
            int xdist = targetPos.x - startPos.x;
            int ydist = targetPos.y - startPos.y;
            int dist = Math.Abs(xdist) + Math.Abs(ydist);
            if (es.faceDir == 0)
            {
                if (xdist > 0 && dist < range) // in face direction
                {
                    return true;
                }
            }
            else if (es.faceDir == 1)
            {
                if (xdist < 0 && dist < range)
                {
                    return true;
                }
            }
            else if (es.faceDir == 2)
            {
                if (ydist > 0 && dist < range)
                {
                    return true;
                }
            }
            else if (es.faceDir == 3)
            {
                if (ydist < 0 && dist < range)
                {
                    return true;
                }
            }
            return false;
        }

        public void changeFaceDir()
        {
            Vector2Int targetPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(target.transform.position);
            Vector2Int startPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
            int xdist = targetPos.x - startPos.x;
            int ydist = targetPos.y - startPos.y;
            if (Math.Abs(xdist) > Math.Abs(ydist))
            {
                if (xdist > 0)
                {
                    es.faceDir = 0;
                }
                else
                {
                    es.faceDir = 1;
                }
            }
            else if (Math.Abs(ydist) > Math.Abs(xdist))
            {
                if (ydist > 0)
                {
                    es.faceDir = 2;
                }
                else
                {
                    es.faceDir = 3;
                }
            }
        }

        public void Chase()
        {
            MapManager map = IngameManager.Instance.mapManager;
            Vector2Int currentpos = map.GetGridPositionFromWorld(gameObject.transform.position);
            map.spots[map.GetGridPositionFromWorld(target.transform.position).x, map.GetGridPositionFromWorld(target.transform.position).y].z = 0;
            Astar astar = new Astar(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height);
            List<Spot> path = astar.CreatePath(map.spots, map.GetGridPositionFromWorld(gameObject.transform.position), map.GetGridPositionFromWorld(target.transform.position), 1000);
            map.spots[map.GetGridPositionFromWorld(target.transform.position).x, map.GetGridPositionFromWorld(target.transform.position).y].z = 1;
            List<Spot> newPath = new List<Spot>();
            path.Reverse();
            map.spots[currentpos.x, currentpos.y].z = 0;
            if (path.Count < es.moveRange)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    newPath.Add(path[i]);
                }
            }
            else
            {
                for (int i = 0; i < es.moveRange; i++)
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

            if (InRange(map.GetGridPositionFromWorld(gameObject.transform.position), map.GetGridPositionFromWorld(target.transform.position), es.maxAttackRange))
            {
                Attack();
            }
        }

        public void Attack()
        {
            target.GetComponent<PlayerState>().OnPlayerHit(es.damage);
            Debug.Log("Attack");
        }
    }
}