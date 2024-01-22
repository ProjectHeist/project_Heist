using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "ScriptableObject/TotalDatabase", order = 1)]
public class Database : ScriptableObject
{
    [SerializeField]
    public PlayerDatabase playerDatabase;
    [SerializeField]
    public MapDatabase mapDatabase;
    [SerializeField]
    public EnemyDatabase enemyDatabase;
    [SerializeField]
    public EXDatabase eXDatabase;
    [SerializeField]
    public WeaponDatabase weaponDatabase;
    [SerializeField]
    public TagDatabase tagDatabase;

}
