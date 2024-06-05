using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Logics;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System.Data;

public enum EnemyPatternType
{
    Guard,
    Patrol,
    Return,
    Chase,
    Alert,
    Lured
}

namespace Ingame
{
    public class EnemyBehaviour : MonoBehaviour
    {
        public EnemyPatternType enemyPattern = EnemyPatternType.Patrol;
        private EnemyState es;
        public EnemyMovement em;
        private int currentPathIndex;
        public int enemyIndex;

        public GameObject suspect; // 시야 내에 들어온 용의자
        public List<GameObject> detectedplayers = new List<GameObject>();
        public int memoryturn = 0; // 적의 기억력. 해당 턴 동안 적은 용의자를 지속해서 추격한다
        public Vector2Int lurePos;
        public GameObject enemyModel;
        public bool actFinished;

        public void CheckAndChangeState() //행동 전에 상태를 보고 현재 상황을 전환 
        {
            List<GameObject> temp = new List<GameObject>();
            for (int i = 0; i < detectedplayers.Count; i++) // 행동을 시작하기 전, 감지된 플레이어들의 상태를 보고, 사망했으면 감지 대상에서 제외
            {
                if (detectedplayers[i].gameObject.activeSelf)
                {
                    temp.Add(detectedplayers[i]);
                }
                else if (detectedplayers[i].gameObject.Equals(suspect)) // 만약 용의자가 사망할 경우, null 처리
                {
                    suspect = null;
                    enemyPattern = EnemyPatternType.Return;
                }
            }
            detectedplayers = temp;

            Detect();
            switch (enemyPattern)
            {
                case EnemyPatternType.Patrol:
                    patrolTransition();
                    break;
                case EnemyPatternType.Return:
                    patrolTransition();
                    break;
                case EnemyPatternType.Chase:
                    chaseTransition();
                    break;
                case EnemyPatternType.Alert:
                    alertTransition();
                    break;
                case EnemyPatternType.Lured:
                    luredTransition();
                    break;
                case EnemyPatternType.Guard:
                    guardTransition();
                    break;
            }
            if (enemyPattern == EnemyPatternType.Alert && !IngameManager.Instance.spawner.policeSpawn)
            {
                IngameManager.Instance.spawner.startspawnTimer(suspect);
            }
        }

        private void guardTransition()
        {

        }

        private void patrolTransition()
        {
            if (suspect != null)
            {
                if (es.suspicion[suspect.GetComponent<PlayerState>().playerIndex] >= 100)
                {
                    memoryturn = 2;
                    enemyPattern = EnemyPatternType.Alert;
                    AlertOthers();
                }
                else if (es.suspicion[suspect.GetComponent<PlayerState>().playerIndex] >= 50)
                {
                    memoryturn = 2;
                    enemyPattern = EnemyPatternType.Chase;
                    AlertOthers();
                }
            }
        }

        private void chaseTransition()
        {
            if (suspect == null) // 용의자가 없어졌을 경우
            {
                memoryturn = 0;
                enemyPattern = EnemyPatternType.Return;
            }
            else if (memoryturn <= 0) // 추격하다가 시야에서 일정 시간 이상 사라졌을 경우
            {
                enemyPattern = EnemyPatternType.Return;
                es.isSuspect[suspect.GetComponent<PlayerState>().playerIndex] = false;
                suspect = null;
            }
            else if (detectedplayers.Contains(suspect)) // 시야 내에 용의자가 있는 경우
            {
                if (es.suspicion[suspect.GetComponent<PlayerState>().playerIndex] >= 100)
                {
                    memoryturn = 2;
                    enemyPattern = EnemyPatternType.Alert;
                    AlertOthers();
                }
                else
                {
                    memoryturn = 2;
                    AlertOthers();
                }
            }
            else
            {
                AlertOthers();
            }
        }

        private void alertTransition()
        {
            if (suspect == null)
            {
                memoryturn = 0;
                enemyPattern = EnemyPatternType.Return;
            }
            else if (memoryturn <= 0)
            {
                enemyPattern = EnemyPatternType.Return;
                es.isSuspect[suspect.GetComponent<PlayerState>().playerIndex] = false;
                suspect = null;
            }
            else if (detectedplayers.Contains(suspect))
            {
                memoryturn = 2;
                AlertOthers();
            }
            else
            {
                AlertOthers();
            }
        }

        private void luredTransition()
        {
            if (suspect == null)
            {
                if (memoryturn <= 0)
                {
                    enemyPattern = EnemyPatternType.Return;
                }
            }
            else
            {
                if (es.suspicion[suspect.GetComponent<PlayerState>().playerIndex] >= 100)
                {
                    memoryturn = 2;
                    enemyPattern = EnemyPatternType.Alert;
                    AlertOthers();
                }
                else if (es.suspicion[suspect.GetComponent<PlayerState>().playerIndex] >= 50)
                {
                    memoryturn = 2;
                    enemyPattern = EnemyPatternType.Chase;
                    AlertOthers();
                }
            }
        }

        private void Detect() // 시야 내에 들어온 아군들의 의심도를 체크, 용의자를 갱신하는 함수
        {
            if (detectedplayers.Count > 0) //감지해서 금지구역에 있는지 확인, 있다면 의심도 증가
            {
                for (int i = 0; i < detectedplayers.Count; i++)
                {
                    Vector2Int currPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(detectedplayers[i].transform.position);
                    int sus = IngameManager.Instance.mapManager.GetSuspicion(currPos); //의심도 체크

                    if (es.wasDetected[detectedplayers[i].GetComponent<PlayerState>().playerIndex])
                    {
                        int idx = detectedplayers[i].GetComponent<PlayerState>().playerIndex;
                        if (sus != 0) // 금지구역에 있을 때 TODO: 다른 조건들도 추가
                        {
                            es.suspicion[idx] += sus;
                            es.wasDetected[idx] = true;
                            es.susIncreased[idx] = true;
                        }
                        else if (enemyPattern == EnemyPatternType.Lured) // 어그로가 끌린 상태에서 플레이어를 발견했을 때
                        {
                            es.suspicion[idx] += 10;
                            es.wasDetected[idx] = true;
                            es.susIncreased[idx] = true;
                        }
                    }

                }

                GameObject max = GetMaxSuspicion(); // 현재 감지된 플레이어 중 가장 큰 의심도를 가진 플레이어를 찾는다
                if (suspect == null)
                {
                    if (es.suspicion[max.GetComponent<PlayerState>().playerIndex] >= 50)
                    {
                        suspect = max;
                    }
                }
                else
                {
                    if (es.suspicion[max.GetComponent<PlayerState>().playerIndex] >= es.suspicion[suspect.GetComponent<PlayerState>().playerIndex])
                    {
                        suspect = max;
                    }
                }
            }
        }

        public GameObject compareSuspect(GameObject origin, GameObject other) // 두 용의자를 비교, 추적에 최적화된 쪽으로 용의자를 변경
        {
            MapManager mm = IngameManager.Instance.mapManager;
            int origindist = mm.GetDist(origin.transform.position, gameObject.transform.position);
            int otherdist = mm.GetDist(other.transform.position, gameObject.transform.position);
            PlayerState psOrigin = origin.GetComponent<PlayerState>();
            PlayerState psOther = other.GetComponent<PlayerState>();
            if (es.suspicion[psOrigin.playerIndex] > es.suspicion[psOther.playerIndex])
            {
                return origin;
            }
            else if (es.suspicion[psOrigin.playerIndex] < es.suspicion[psOther.playerIndex])
            {
                return other;
            }
            else if (origindist > otherdist)
            {
                return other;
            }
            else if (origindist < otherdist)
            {
                return origin;
            }
            else if (origin.GetComponent<PlayerState>().HP > other.GetComponent<PlayerState>().HP)
            {
                return other;
            }
            else if (origin.GetComponent<PlayerState>().HP < other.GetComponent<PlayerState>().HP)
            {
                return origin;
            }
            else
            {
                return origin;
            }
        }

        public void AlertOthers() //특정 상황에서 의심도와 현재 상태를 주변 적들에게 전파
        {
            List<GameObject> enemies = IngameManager.Instance.enemies;
            em = gameObject.GetComponent<EnemyMovement>();
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyBehaviour eb = enemies[i].GetComponent<EnemyBehaviour>();
                Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
                Vector2Int targetPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(enemies[i].transform.position);
                if (em.InRange(currentPos, targetPos, gameObject.GetComponent<EnemyState>().alertRange))
                {
                    if (eb.suspect != null)
                        eb.suspect = compareSuspect(eb.suspect, suspect);
                    else
                        eb.suspect = suspect;

                    int currentsuspicion = es.suspicion[suspect.GetComponent<PlayerState>().playerIndex];
                    if (currentsuspicion >= 100) // Alert
                    {
                        enemies[i].GetComponent<EnemyBehaviour>().enemyPattern = EnemyPatternType.Alert;
                        es.SetSuspicion(true, eb.suspect.GetComponent<PlayerState>().playerIndex, currentsuspicion);
                    }
                    else if (currentsuspicion >= 50) // Chase
                    {
                        enemies[i].GetComponent<EnemyBehaviour>().enemyPattern = EnemyPatternType.Chase;
                        es.SetSuspicion(true, eb.suspect.GetComponent<PlayerState>().playerIndex, currentsuspicion);
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

        public void EnemyAct() //적의 실제 행동
        {
            actFinished = false;
            es = gameObject.GetComponent<EnemyState>();
            em = gameObject.GetComponent<EnemyMovement>();
            MapManager map = IngameManager.Instance.mapManager;

            CheckAndChangeState(); // 상황에 따라 현재 적의 상태를 조절
            memoryCount();
            Debug.Log(enemyPattern);

            switch (enemyPattern)
            {
                case EnemyPatternType.Guard:
                    break;
                case EnemyPatternType.Patrol:
                    gameObject.GetComponent<EnemyPatrol>().Patrol();
                    break;
                case EnemyPatternType.Return:
                    gameObject.GetComponent<EnemyPatrol>().ReturnToPatrol();
                    break;
                case EnemyPatternType.Chase:
                    gameObject.GetComponent<EnemyChase>().Chase(map.GetGridPositionFromWorld(suspect.transform.position));
                    em.changeFaceDir(map.GetGridPositionFromWorld(suspect.transform.position));
                    Debug.Log("Chase");
                    break;
                case EnemyPatternType.Alert:
                    gameObject.GetComponent<EnemyAlert>().AlertAndAttack(map.GetGridPositionFromWorld(suspect.transform.position));
                    em.changeFaceDir(map.GetGridPositionFromWorld(suspect.transform.position));
                    Debug.Log("Alert");
                    break;
                case EnemyPatternType.Lured:
                    gameObject.GetComponent<EnemyLured>().Lured(lurePos);
                    em.changeFaceDir(lurePos);
                    Debug.Log("Lured");
                    break;
            }
            Debug.Log(enemyPattern);
        }

        public GameObject GetMaxSuspicion()
        {
            es = gameObject.GetComponent<EnemyState>();
            int maxVal = -1;
            int maxIdx = -1;
            if (suspect != null)
            {
                maxVal = es.suspicion[suspect.GetComponent<PlayerState>().playerIndex];
                for (int i = 0; i < detectedplayers.Count; i++)
                {
                    int sus = es.suspicion[detectedplayers[i].GetComponent<PlayerState>().playerIndex];
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
                    return suspect;
                }
            }
            else
            {
                for (int i = 0; i < detectedplayers.Count; i++)
                {
                    int sus = es.suspicion[detectedplayers[i].GetComponent<PlayerState>().playerIndex];
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
}