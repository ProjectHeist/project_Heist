using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static IngameManager instance = null;
    public MapCreator mapCreator;
    public MapManager mapManager = new MapManager();
    public PlayerController playerController = new PlayerController();
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

    public void init()
    {
        if (null == instance)
        {
            instance = this;
        }
        mapCreator.init();
        tags = GetTags();
        extractionPoint = mapManager.GetGridCellFromPosition(new Vector2Int(0, 0)).GetComponent<GridCell>();
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
            if (extractedplayers >= players.Count)
            {
                Debug.Log("End Extraction");
            }
            else
            {
                Debug.Log("Start Extraction");
                extractionPoint.CheckPlayer();
                if (extractionPoint.playerInThisGrid != null)
                {
                    extractionPoint.playerInThisGrid.SetActive(false);
                    extractedplayers++;
                }
            }

        }
    }

    public void EndGame()
    {
        // end game and change scene if all players are extracted
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

}
