using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static IngameManager instance = null;
    public MapCreator mapCreator;
    public MapManager mapManager = new MapManager();
    public PlayerController playerController = new PlayerController();
    public Transform mainCamera;
    public ControlPanel controlPanel;
    public InputManager InputManager;
    public List<GameObject> Prefabs;
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

    void OnEnable()
    {
        GameManager.Instance._ingame = this;
        init();
    }

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
            if (ps.InteractionTime > 0) // 상호작용 중이면 못움직임
            {
                ps.InteractionTime--;
            }
            else
            {
                ps.canMove = 1;
                ps.canAttack = 1;
                controlPanel.Move.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
                controlPanel.Attack.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
            }

            if (ps.EXcooldown != 0)
            {
                ps.EXcooldown--;
                controlPanel.EX.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
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
