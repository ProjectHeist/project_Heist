using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    public abstract class PlayerEX : ScriptableObject
    {
        public int skillNo;
        public int duration = 0; //지속시간
        public int coolTime;
        public int range = -1;
        public string skillname;
        public string skillcontent;
        public int subject = -1; // 플레이어 대상일시 0, 적 대상일시 1, 필요 없을 시 -1 
        public GameObject target; //타깃
        public Vector2Int targetPosition;
        public virtual void UseEX(PlayerState ps)
        {

        }
    }
}

