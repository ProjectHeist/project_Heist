using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using Ingame;

namespace Logics
{
    public class IngameManager : MonoBehaviour
    {
        // Start is called before the first frame update
        private static IngameManager instance = null;
        public MapCreator mapCreator;
        public MapManager mapManager = new MapManager();
        public PlayerController playerController = new PlayerController();
        public Transform mainCamera;
        public ControlUI controlPanel;
        public InputManager InputManager;
        public List<GameObject> Prefabs;
        public IngameUIManager ingameUI;
        public bool ObjectiveCleared = false;
        public int TotalMoney = 0;
        public bool Deployed = false;
        public int deployedplayers = 0;
        public int extractedplayers = 0;
        public GridCell extractionPoint;

        public static IngameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    return null;
                }
                return instance;
            }
        }

        public int turn = 1;
        public int phase = 0; // 0 is player's phase, 1 is enemy's phase

        //-------------------PlayerList and Enemies--------------------//
        public List<GameObject> players;
        public List<GameObject> enemies;
        public List<string> tags;
        //-------------------------------------------------------------//

        void Start() // 테스트용
        {
            //GameManager.Instance._ingame = this;
            init();
        }

        /*void OnEnable() //실제 사용
        {
            GameManager.Instance._ingame = this;
            init();
        }*/


        public void init()
        {
            if (null == instance)
            {
                instance = this;
            }
            mapCreator.init();
            tags = GetTags();
            extractionPoint = mapManager.GetGridCellFromPosition(new Vector2Int(0, 0)).GetComponent<GridCell>();
            turn = 1;
            CreatePlayerList();
            CreateEnemy();
            ingameUI.Init();
            StartPlayerPhase();
        }

        public void EnemyPhase() // 페이즈 종료 버튼 눌렀을 때 
        {
            CheckObjective();
            phase = 1;
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].GetComponent<EnemyBehaviour>().EnemyAct();
            }

            phase = 0;
            turn++;
            StartPlayerPhase();
        }

        public void StartPlayerPhase() //플레이어의 페이즈일 때 모든 플레이어에 대해 돌면서 작용함
        {
            foreach (GameObject player in players)
            {
                PlayerState ps = player.GetComponent<PlayerState>();
                if (ps.StateChangeList.Count > 0) //플레이어에게 적용된 버프를 확인 및 해제하는 절차
                {
                    List<BuffInfo> filter = new List<BuffInfo>();
                    foreach (BuffInfo buff in ps.StateChangeList)
                    {
                        if (buff.duration == 1) //버프 시간이 1턴밖에 남지 않은 경우
                        {
                            switch (buff.stat) //버프 종류에 따라 원래대로 돌려놓고 제거
                            {
                                case stats.accuracy:
                                    ps.accuracy -= buff.value;
                                    break;
                                case stats.critRate:
                                    ps.critRate -= buff.value;
                                    break;
                                case stats.damage:
                                    ps.damage -= Convert.ToInt32(buff.value);
                                    break;
                                case stats.attackRange:
                                    ps.maxAttackRange -= Convert.ToInt32(buff.value);
                                    break;
                            }
                        }
                        else
                        {
                            buff.duration--;
                            filter.Add(buff);
                        }
                    }
                    ps.StateChangeList = filter;
                }

                if (ps.InteractionTime > 0) // 상호작용 중이면 못움직임
                {
                    ps.InteractionTime--;
                }
                else // 다시 이동&공격 가능한 상태로 전환
                {
                    ps.canMove = 1;
                    ps.canAttack = 1;
                    ps.remainMoveRange = ps.moveRange;
                    ingameUI.IsSelected(PanelType.Attack, true);
                    ingameUI.IsSelected(PanelType.Move, true);
                }

                if (ps.EXcooldown != 0) // EX 쿨타임 감소
                {
                    ps.EXcooldown--;
                    ingameUI.IsSelected(PanelType.EX, true);
                }
            }
        }

        public void Deploy(Vector2Int deployPos)
        {
            Vector3 deploy = mapManager.GetWorldPositionFromGridPosition(deployPos);
            if (mapManager.spots[deployPos.x, deployPos.y].z == 0) // 벽 소환 방지
            {
                players[deployedplayers].transform.position = deploy;
                players[deployedplayers].SetActive(true);
                mapManager.spots[deployPos.x, deployPos.y].z = 1;
                deployedplayers++;
                if (deployedplayers >= players.Count)
                {
                    Deployed = true;
                }
            }
        }

        public void CheckObjective()
        {
            //check objective and decide ending game
            if (TotalMoney >= 20000)
            {
                ObjectiveCleared = true;
            }
            if (ObjectiveCleared)
            {
                Debug.Log("Start Extraction");
                extractionPoint.CheckPlayer();
                if (extractionPoint.playerInThisGrid != null)
                {
                    extractionPoint.playerInThisGrid.transform.position = new Vector3(-100, -100, -100);
                    extractionPoint.playerInThisGrid.SetActive(false);
                    mapManager.spots[extractionPoint.posX, extractionPoint.posY].z = 0;
                    extractionPoint.playerInThisGrid = null;
                    extractedplayers++;
                }
                if (extractedplayers >= players.Count)
                {
                    Debug.Log("End Extraction");
                    EndGame();
                }
            }
        }

        public void EndGame()
        {
            // end game and change scene if all players are extracted
            GameManager.Instance.currentMaster.money += TotalMoney;
            SceneManager.LoadScene("LobbyScene");
        }

        public List<string> GetTags()
        {
            List<string> tags = new List<string>();
            for (int i = 0; i < GameManager.Instance._data.tagList.tag.Length; i++)
            {
                tags.Add(GameManager.Instance._data.tagList.tag[i]);
            }
            return tags;
        }

        void CreatePlayerList()
        {
            //int[] playerlist = GameManager.Instance.playerIndex; //테스트 시 이 부분은 주석 처리 
            //int[] weaponlist = GameManager.Instance.weaponIndex; //테스트 시 이 부분은 주석 처리 

            for (int i = 0; i < 3; i++) //테스트용 리스트 
            {
                PlayerStat ps = GameManager.Instance._data.playerDatabase.totalPlayerStat.playerStats[i];
                WeaponStat ws = GameManager.Instance._data.weaponDatabase.weaponStatList.weaponStats[i];
                GameObject player = Instantiate(Prefabs[0]);
                player.GetComponent<PlayerState>().SetState(ps, ws);
                player.GetComponentInChildren<HPBar>().SetMaxHealth(player.GetComponent<PlayerState>().HP);

                players.Add(player);
                player.SetActive(false);
            }
            /*for (int i = 0; i < playerlist.Length; i++) //테스트 시 이 부분은 주석 처리
            {
                if (playerlist[i] != -1) // 플레이어를 해당 슬롯에 편성했을 시
                {
                    WeaponStat ws;
                    if (weaponlist[i] != -1)
                    {
                        ws = GameManager.Instance._data.weaponDatabase.weaponStatList.weaponStats[weaponlist[i]]; // 무장 시
                        Debug.Log(ws.weaponDamage);
                    }
                    else
                    {
                        ws = new WeaponStat(); // 비무장 시
                    }
                    PlayerStat ps = GameManager.Instance._data.playerDatabase.totalPlayerStat.playerStats[playerlist[i]];
                    Debug.Log(ps.playerMoveRange);

                    GameObject player = Instantiate(Prefabs[0]);
                    player.GetComponent<PlayerState>().SetState(ps, ws);
                    player.GetComponentInChildren<HPBar>().SetMaxHealth(player.GetComponent<PlayerState>().HP);

                    players.Add(player);
                    player.SetActive(false);
                }
            }*/
        }

        void CreateEnemy()
        {
            MapData mapdata = GameManager.Instance._data.mapDatabase.testData;
            for (int i = 0; i < mapdata.EnemyNum; i++)
            {
                Vector3 spawnPos = mapManager.GetWorldPositionFromGridPosition(new Vector2Int(mapdata.enemyPos[i].x, mapdata.enemyPos[i].y));
                mapManager.spots[mapdata.enemyPos[i].x, mapdata.enemyPos[i].y].z = 1;
                GameObject enemy = Instantiate(Prefabs[1], spawnPos, Quaternion.identity);
                enemy.GetComponent<EnemyState>().GetEnemyInfo();
                HPBar hp = enemy.GetComponentInChildren<HPBar>();
                hp.SetMaxHealth(enemy.GetComponent<EnemyState>().HP);
                enemies.Add(enemy);
            }
        }

    }
}