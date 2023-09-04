using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    // Start is called before the first frame update
    public void Interact(GameObject _object, int dist)
    {
        switch (_object.tag)
        {
            case "Door":
                _object.GetComponent<Door>().OnDoorInteracted();
                break;
            case "Money":
                _object.GetComponent<Money>().OnMoneyInteracted();
                break;

        }
    }

}
