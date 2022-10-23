using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionStorage", menuName = "Configs/ExplosionStorage", order = 0)]
public class ExplosionStorage : ScriptableObject
{
    // Info from ExplosionEvent.Size
    [Header("0 - Small, 1 - Medium, 2 - Large")]
    public GameObject[] ExplosionPrefab;
}
