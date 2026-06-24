using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTileColor : MonoBehaviour
{
    public void SetTileStyle(TileStyle style)
    {
        Color color = style.color;
        color.a = style.opacity;
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }
}
