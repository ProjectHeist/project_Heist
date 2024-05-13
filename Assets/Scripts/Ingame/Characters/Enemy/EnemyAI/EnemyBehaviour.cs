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
    Return,
    Chase,
    Alert,
    Lured
}

namespace Ingame
{
    public class EnemyBehaviour : MonoBehaviour
    {
        public EnemyPattern enemyPattern = EnemyPattern.Patrol;
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
                    enemyPattern = EnemyPattern.Return;
                }
            }
            detectedplayers = temp;

            Detect();
            switch (enemyPattern)
            {
                case EnemyPattern.Patrol:
                    patrolTransition();
                    break;
                case EnemyPattern.Return:
                    patrolTransition();
                    break;
                case EnemyPattern.Chase:
                    chaseTransition();
                    break;
                case EnemyPattern.Alert:
                    alertTransition();
                    break;
                case EnemyPattern.Lured:
                    luredTransition();
                    break;
                case EnemyPattern.Guard:
                    guardTransition();
                    break;
            }
        }

        private void guardTransition()
        {

        }

        private void patrolTransition()
        {
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
                    enemyPattern = EnemyPattern.Chase;
                    AlertOthers();
                }
            }
        }

        private void chaseTransition()
        {
            if (suspect == null) // 용의자가 없어졌을 경우
            {
                memoryturn = 0;
                enemyPattern = EnemyPattern.Return;
            }
            else if (memoryturn <= 0) // 추격하다가 시야에서 일정 시간 이상 사라졌을 경우
            {
                enemyPattern = EnemyPattern.Return;
                suspect.GetComponent<PlayerState>().isSuspect[enemyIndex] = false;
                suspect = null;
            }
            else if (detectedplayers.Contains(suspect)) // 시야 내에 용의자가 있는 경우
            {
                if (suspect.GetComponent<PlayerState>().suspicion[enemyIndex] >= 100)
                {
                    memoryturn = 2;
                    enemyPattern = EnemyPattern.Alert;
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
                enemyPattern = EnemyPattern.Return;
            }
            else if (memoryturn <= 0)
            {
                enemyPattern = EnemyPattern.Return;
                suspect.GetComponent<PlayerState>().isSuspect[enemyIndex] = false;
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
                    enemyPattern = EnemyPattern.Return;
                }
            }
            else
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
                    enemyPattern = EnemyPattern.Chase;
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
                    if (!detectedplayers[i].GetComponent<PlayerState>().wasDetected[enemyIndex])
                    {
                        if (sus != 0) // 금지구역에 있을 때 TODO: 다른 조건들도 추가
                        {
                            detectedplayers[i].GetComponent<PlayerState>().suspicion[enemyIndex] += sus;
                            detectedplayers[i].GetComponent<PlayerState>().wasDetected[enemyIndex] = true;
                            detectedplayers[i].GetComponent<PlayerState>().susIncreased[enemyIndex] = true;
                        }
                        else if (enemyPattern == EnemyPattern.Lured) // 어그로가 끌린 상태에서 플레이어를 발견했을 때
                        {
                            detectedplayers[i].GetComponent<PlayerState>().suspicion[enemyIndex] += 10;
                            detectedplayers[i].GetComponent<PlayerState>().wasDetected[enemyIndex] = true;
                            detectedplayers[i].GetComponent<PlayerState>().susIncreased[enemyIndex] = true;
                        }
                    }

                }

                GameObject max = GetMaxSuspicion(); // 현재 감지된 플레이어 중 가장 큰 의심도를 가진 플레이어를 찾는다
                if (suspect == null)
                {
                    if (max.GetComponent<PlayerState>().suspicion[enemyIndex] >= 50)
                    {
                        suspect = max;
                    }
                }
                else
                {
                    if (max.GetComponent<PlayerState>().suspicion[enemyIndex] >= suspect.GetComponent<PlayerState>().suspicion[enemyIndex])
                    {
                        suspect = max;
                    }
                }
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
                case EnemyPattern.Guard:
                    break;
                case EnemyPattern.Patrol:
                    gameObject.GetComponent<EnemyPatrol>().Patrol();
                    break;
                case EnemyPattern.Return:
                    gameObject.GetComponent<EnemyPatrol>().ReturnToPatrol();
                    break;
                case EnemyPattern.Chase:
                    gameObject.GetComponent<EnemyChase>().Chase(map.GetGridPositionFromWorld(suspect.transform.position));
                    em.changeFaceDir(map.GetGridPositionFromWorld(suspect.transform.position));
                    Debug.Log("Chase");
                    break;
                case EnemyPattern.Alert:
                    gameObject.GetComponent<EnemyAlert>().AlertAndAttack(map.GetGridPositionFromWorld(suspect.transform.position));
                    em.changeFaceDir(map.GetGridPositionFromWorld(suspect.transform.position));
                    Debug.Log("Alert");
                    break;
                case EnemyPattern.Lured:
                    gameObject.GetComponent<EnemyLured>().Lured(lurePos);
                    em.changeFaceDir(lurePos);
                    Debug.Log("Lured");
                    break;
            }
            Debug.Log(enemyPattern);
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