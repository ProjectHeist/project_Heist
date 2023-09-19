using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEX : ScriptableObject
{
    public int skillNo;
    public int duration = 0; //지속시간
    public int coolTime;
    public int range = -1;
    public string skillname;
    public string skillcontent;
    public virtual void UseEX(PlayerState ps)
    {

    }
}
