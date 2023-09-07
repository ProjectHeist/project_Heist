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

    public int turn;   // 0 is player's turn, 1 is enemy's turn
    public int phase;

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
        turn = 0;
        phase = 1;
    }

    public void EnemyTurn()
    {
        turn = 1;
        // Methods For enemies, Controled by IngameManager

        turn = 0;
        phase++;
        OnTurnChange();
    }

    public void OnTurnChange()
    {
        foreach (GameObject player in players)
        {
            PlayerState ps = player.GetComponent<PlayerState>();
            ps.canMove = 1;
            ps.canAttack = 1;
        }
    }

    public void Extraction()
    {
        //set Extraction point if certain missions are accomplished
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
