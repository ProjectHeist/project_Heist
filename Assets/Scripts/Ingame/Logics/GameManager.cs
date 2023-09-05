using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public DataManager _data = new DataManager();
    public UIManager _ui = new UIManager();
    public IngameManager _ingame;


    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        init();
    }

    public static GameManager Instance
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

    // Update is called once per frame
    void Update()
    {

    }

    void init()
    {
        _data.init();
        _ingame.init();
    }

    void selectPlayer()
    {

    }
}
