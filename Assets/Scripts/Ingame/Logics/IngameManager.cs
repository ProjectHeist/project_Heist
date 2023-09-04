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

    public List<GameObject> players;
    public List<GameObject> enemies;
    public List<string> tags = new List<string>();

    void Start()
    {
        tags.Add("Door");
        tags.Add("Wall");
        tags.Add("Computer");
        tags.Add("Money");
        tags.Add("Vent");
        for (int i = 0; i < tags.Count; i++)
        {
            Debug.Log(tags[i]);
        }
    }

    public void init()
    {
        if (null == instance)
        {
            instance = this;
        }
        mapCreator.init();
    }
}
