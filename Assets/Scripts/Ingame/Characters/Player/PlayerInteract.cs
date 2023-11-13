using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    // Start is called before the first frame update
    public void Interact(GameObject _object, int dist)
    {
        switch (_object.tag)
        {
            case "Door":
                CheckInteractionObj(_object);
                break;
            case "Money":
                _object.GetComponent<Money>().OnMoneyInteracted();
                break;

        }
    }

    public void CheckInteractionObj(GameObject gobject)
    {
        PlayerState ps = gameObject.GetComponent<PlayerState>();
        Door d = gobject.GetComponent<Door>();
        if (d.requiredTime > 0 && !ps.isInteracting)
        {
            ps.isInteracting = true;
            ps.InteractionTime = d.requiredTime;
            ps.canAttack = 0;
            ps.canMove = 0;
        }
        else if (ps.InteractionTime == 0)
        {
            d.OnDoorInteracted();
            ps.isInteracting = false;
        }
    }

}
