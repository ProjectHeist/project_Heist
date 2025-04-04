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
        public int playerIndex;
        public bool isImmune = false;

        public List<GameObject> enemyDetectedPlayer = new List<GameObject>(); // 현재 캐릭터를 감지한 적
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
            if (!isImmune)
                OnCharacterHit(damage);
        }

        public void OnPlayerDeath()
        {
            IngameManager.Instance.spawner.stopSpawnTimer();
        }

        public void DecreaseSuspicion()
        {
            for (int i = 0; i < IngameManager.Instance.enemies.Count; i++)
            {
                EnemyState es = IngameManager.Instance.enemies[i].GetComponent<EnemyState>();
                if (!es.isSuspect[playerIndex] && es.suspicion[playerIndex] < 50 && !es.susIncreased[playerIndex])
                {
                    es.suspicion[playerIndex] -= 20;
                    if (es.suspicion[playerIndex] <= 0)
                    {
                        es.suspicion[playerIndex] = 0;
                    }
                }
            }
        }
    }
}



