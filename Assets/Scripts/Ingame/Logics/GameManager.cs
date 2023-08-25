using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    private static GameManager instance = null;
    public DataManager _data = new DataManager();
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
    }
}
