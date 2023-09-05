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
