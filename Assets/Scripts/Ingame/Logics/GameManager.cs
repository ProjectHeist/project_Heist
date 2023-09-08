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
        StartGame();
    }

    void StartGame()
    {
        //Change Scene and get IngameManager, and do things
        _ingame.init();
        CreatePlayerList();
    }

    void SelectPlayers()
    {
        // 출격시킬 캐릭터와 무기를 선택하는 함수, 결정 시 아래로 넘어감.
    }

    void CreatePlayerList()
    {
        int[] testplayerlist = { 1, 0 };
        int[] testweaponlist = { 0, 1 };
        for (int i = 0; i < testplayerlist.Length; i++)
        {
            PlayerStat ps = _data.playerDatabase.totalPlayerStat.playerStats[testplayerlist[i]];
            Debug.Log(ps.playerMoveRange);
            WeaponStat ws = _data.weaponDatabase.weaponStatList.weaponStats[testweaponlist[i]];
            Debug.Log(ws.weaponDamage);
            GameObject player = Instantiate(_ingame.Prefabs[0]);
            player.GetComponent<PlayerState>().SetState(ps, ws);
            _ingame.players.Add(player);
            player.SetActive(false);
        }
    }
}
