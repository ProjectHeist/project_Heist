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
        public MapManager mapManager;
        public PlayerController playerController = new PlayerController();
        public Walldetection walldetection = new Walldetection();
        public Spawner spawner;
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
        public bool isEnemyPhase;


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
            mapManager = new MapManager(GameManager.Instance._data.totalDB.mapDatabase.MapDataList[GameManager.Instance.mapIndex]);
            if (null == instance)
            {
                instance = this;
            }
            mapCreator.init();
            mapManager.initGrid();
            tags = GetTags();
            extractionPoint = mapManager.GetGridCellFromPosition(new Vector2Int(0, 0)).GetComponent<GridCell>();
            turn = 1;

            spawner = new Spawner(mapManager, Prefabs[1]);
            CreatePlayerList();
            EnemyInit();
            ingameUI.Init();
            StartPlayerPhase();
        }

        public IEnumerator EnemyPhase() // 페이즈 종료 버튼 눌렀을 때 
        {
            isEnemyPhase = true;
            spawner.spawnPolice();
            CheckObjective();
            phase = 1;
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].GetComponent<EnemyBehaviour>().EnemyAct();
                yield return new WaitUntil(() => enemies[i].GetComponent<EnemyBehaviour>().actFinished);
            }
            phase = 0;
            turn++;
            StartPlayerPhase();
        }

        public void StartPlayerPhase() //플레이어의 페이즈일 때 모든 플레이어에 대해 돌면서 작용함
        {
            isEnemyPhase = false;
            foreach (GameObject player in players)
            {
                PlayerState ps = player.GetComponent<PlayerState>(); // 의심도 50 이하에서 용의자가 아닐 시 의심도가 떨어짐
                ps.DecreaseSuspicion();
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
            foreach (GameObject enemy in enemies)
            {
                EnemyState es = enemy.GetComponent<EnemyState>();
                for (int i = 0; i < es.wasDetected.Count; i++)
                {
                    es.wasDetected[i] = false;
                    es.susIncreased[i] = false;
                }
            }
        }

        public void Deploy(Vector2Int deployPos)
        {
            Vector3 deploy = mapManager.GetWorldPositionFromGridPosition(deployPos);
            if (mapManager.spots[deployPos.x, deployPos.y].z == 0) // 벽 소환 방지
            {
                PlayerState ps = players[deployedplayers].GetComponent<PlayerState>();
                players[deployedplayers].transform.position = deploy;
                players[deployedplayers].SetActive(true);
                mapManager.spots[deployPos.x, deployPos.y].z = 1;

                SetDir(ps.faceDir, ps);

                deployedplayers++;
                if (deployedplayers >= players.Count)
                {
                    Deployed = true;
                }
            }
        }

        public void SetDir(int dir, PlayerState playerState)
        {
            switch (dir)
            {
                case 0:
                    playerState.playerModel.transform.eulerAngles = new Vector3(0, 90, 0);
                    break;
                case 1:
                    break;
                case 2:
                    playerState.playerModel.transform.eulerAngles = new Vector3(0, -90, 0);
                    break;
                case 3:
                    playerState.playerModel.transform.eulerAngles = new Vector3(0, -180, 0);
                    break;
            }
        }

        public void CheckObjective()
        {
            //check objective and decide ending game
            if (players.Count == 0)
            {
                EndGameDefeat();
            }
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

        public void EndGameDefeat()
        {
            SceneManager.LoadScene("LobbyScene");
        }

        public List<string> GetTags()
        {
            List<string> tags = new List<string>();
            for (int i = 0; i < GameManager.Instance._data.totalDB.tagDatabase.tags.Length; i++)
            {
                tags.Add(GameManager.Instance._data.totalDB.tagDatabase.tags[i]);
            }
            return tags;
        }

        void CreatePlayerList()
        {
            //int[] playerlist = GameManager.Instance.playerIndex; //테스트 시 이 부분은 주석 처리 
            //int[] weaponlist = GameManager.Instance.weaponIndex; //테스트 시 이 부분은 주석 처리 

            for (int i = 0; i < 3; i++) //테스트용 리스트 
            {
                PlayerStat ps = GameManager.Instance._data.totalDB.playerDatabase.playerStatList[i];
                WeaponStat ws = GameManager.Instance._data.totalDB.weaponDatabase.weaponStatList[i];
                GameObject player = Instantiate(Prefabs[0]);
                player.GetComponent<PlayerState>().SetState(ps, ws, 0);
                player.GetComponent<PlayerState>().playerIndex = i;
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

        public void EnemyInit() // 시작할 때의 적
        {
            MapData mapdata = GameManager.Instance._data.totalDB.mapDatabase.MapDataList[GameManager.Instance.mapIndex];
            for (int i = 0; i < mapdata.EnemyNum; i++)
            {
                Vector2Int startPos = mapdata.enemyPos[i];
                if (i < 2)
                    spawner.CreateEnemy(i, i, startPos);
                else
                    spawner.CreateEnemy(i, 2, startPos);
            }
        }

        public void EnemyAdd()
        {
            MapData mapdata = GameManager.Instance._data.totalDB.mapDatabase.MapDataList[GameManager.Instance.mapIndex];
            for (int i = 0; i < mapdata.spawnEnemyNum; i++)
            {
                Vector2Int startPos = mapdata.spawnPos[i];
                if (i < 2)
                    spawner.CreateEnemy(i, i, startPos);
                else
                    spawner.CreateEnemy(i, 2, startPos);
            }
        }



        /*void CreateEnemy()
        {
            MapData mapdata = GameManager.Instance._data.totalDB.mapDatabase.MapDataList[GameManager.Instance.mapIndex];
            for (int i = 0; i < mapdata.EnemyNum; i++)
            {
                Vector3 initPos = mapManager.GetWorldPositionFromGridPosition(new Vector2Int(mapdata.enemyPos[i].x, mapdata.enemyPos[i].y));
                mapManager.spots[mapdata.enemyPos[i].x, mapdata.enemyPos[i].y].z = 1;
                GameObject enemy = Instantiate(Prefabs[1], initPos, Quaternion.identity);
                enemy.GetComponent<EnemyState>().GetEnemyInfo();
                enemy.GetComponent<EnemyBehaviour>().enemyIndex = i;
                if (i < 2)
                    enemy.GetComponent<EnemyState>().routeNum = i;
                else
                    enemy.GetComponent<EnemyState>().routeNum = 2;
                HPBar hp = enemy.GetComponentInChildren<HPBar>();
                hp.SetMaxHealth(enemy.GetComponent<EnemyState>().HP);
                CreateEnemyVision(enemy.GetComponent<EnemyState>().detectRange, enemy.transform.GetChild(2).gameObject);
                enemies.Add(enemy);
            }
        }

        void CreateEnemyVision(int range, GameObject vision)
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

        }*/

    }
}