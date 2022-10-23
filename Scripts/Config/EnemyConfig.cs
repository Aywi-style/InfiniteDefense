using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/EnemyConfig", order = 0)]
public class EnemyConfig : ScriptableObject
{
    public GameObject ShipPrefab;
    public GameObject EnemyPrefab;
    public GameObject RangeEnemyPrefab;

}
