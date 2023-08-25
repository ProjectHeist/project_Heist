using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    DataManager _data = new DataManager();
    void Awake()
    {
        _data.init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    static void init()
    {

    }
}
