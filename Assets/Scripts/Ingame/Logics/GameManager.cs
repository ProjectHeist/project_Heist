using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public MasterData currentMaster;
    public DataManager _data = new DataManager();
    public UIManager _ui = new UIManager();
    public IngameManager _ingame;
    public int[] playerIndex;
    public int[] weaponIndex;


    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
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
    }

    public void StartGame(string mapName, int[] playerList, int[] weaponList)
    {
        playerIndex = playerList;
        weaponIndex = weaponList;
        SceneManager.LoadScene(mapName);
        //Change Scene and get IngameManager, and do things
    }


}
