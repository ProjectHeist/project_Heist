using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTileColor : MonoBehaviour
{
    public void SetColor(Color color)
    {
        color.a = 0.5f;
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }
}
