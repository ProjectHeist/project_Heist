using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Logics;
using Unity.VisualScripting;
using UnityEngine;


namespace Ingame
{
    public class PlayerState : CharacterState
    {
        public int EXIndex;
        public int EXcooldown; // 현재 남아있는 쿨타임
        private int EXcooltime; // 쿨타임
        public int remainMoveRange; //이동 가능한 남은 거리
        public List<int> suspicion = new List<int>(); // 적별 의심도
        public List<bool> isSuspect = new List<bool>(); // 적별 의심되는 상태 
        public List<bool> isDetected = new List<bool>(); // 적 턴 동안 이미 감지된 적이 있는가?
        public GameObject playerModel;
        public int faceDir; //0 is +x, 1 is +y, 2 is -x, 3 is -y
        public int soundRange;

        public PlayerState()
        {
            canAttack = 1;
            canMove = 1;
            EXcooldown = 0;
        }
        public void SetState(PlayerStat playerStat, WeaponStat weaponStat, int Dir)
        {
            Number = playerStat.playerNumber;
            Name = playerStat.playerName;
            HP = playerStat.playerHP;
            maxHP = playerStat.playerHP;
            accuracy = playerStat.playerAccuracy;
            moveRange = playerStat.playerMoveRange;
            damage = weaponStat.weaponDamage;
            maxAttackRange = weaponStat.weaponMaxAttackRange;
            minAttackRange = weaponStat.weaponMinAttackRange;
            critRate = weaponStat.weaponCritRate;
            EXIndex = playerStat.PlayerEX;
            faceDir = Dir;
            soundRange = weaponStat.soundRange;
            for (int i = 0; i < IngameManager.Instance.enemies.Count; i++)
            {
                suspicion.Add(0);
                isSuspect.Add(false);
                isDetected.Add(false);
            }
        }

        private int Number; // 도감에 존재하는 플레이어 넘버
        [SerializeField]
        private string Name; // 플레이어의 실제 이름 
        private int Class; // 플레이어의 클래스
        public int InteractionTime = 0; //상호작용 중인 시간 
        public bool isInteracting = false;

        public int playerNumber
        {
            get
            {
                return Number;
            }
        }
        public string playerName
        {
            get
            {
                return Name;
            }
        }
        public int playerClass
        {
            get
            {
                return Class;
            }
        }
        public void OnPlayerHit(int damage)
        {
            OnCharacterHit(damage);
        }

        public void SetSuspicion(int index, int sus)
        {
            suspicion[index] = sus;
        }

        public void DecreaseSuspicion()
        {
            for (int i = 0; i < suspicion.Count; i++)
            {
                if (!isSuspect[i] && suspicion[i] < 50)
                {
                    suspicion[i] -= 20;
                    if (suspicion[i] <= 0)
                    {
                        suspicion[i] = 0;
                    }
                }
            }
        }
    }
}



