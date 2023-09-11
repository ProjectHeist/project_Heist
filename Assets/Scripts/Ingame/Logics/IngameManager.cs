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
    }

    public void EnemyPhase() // 페이즈 종료 버튼 눌렀을 때 
    {
        CheckObjective();
        phase = 1;
        // Methods For enemies, Controled by IngameManager

        phase = 0;
        turn++;
        StartPlayerPhase();
    }

    public void StartPlayerPhase()
    {
        foreach (GameObject player in players)
        {
            PlayerState ps = player.GetComponent<PlayerState>();
            ps.canMove = 1;
            ps.canAttack = 1;
            controlPanel.Move.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
            controlPanel.Attack.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        }
    }

    public void Deploy(Vector2Int deployPos)
    {
        Vector3 deploy = mapManager.GetWorldPositionFromGridPosition(deployPos);
        players[deployedplayers].transform.position = deploy;
        players[deployedplayers].SetActive(true);
        deployedplayers++;
        if (deployedplayers >= players.Count)
        {
            Deployed = true;
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
        int[] playerlist = GameManager.Instance.playerIndex;
        int[] weaponlist = GameManager.Instance.weaponIndex;
        for (int i = 0; i < playerlist.Length; i++)
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
                players.Add(player);
                player.SetActive(false);
            }
        }
    }

}
