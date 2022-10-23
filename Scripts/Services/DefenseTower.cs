using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenseTower
{
    public int LevelTower;
    public int TowerHealth;
    public GameObject TowerPrefab;
    public Mesh TowerMesh;
    public GameObject CannonBallPrefab;
    public int Radius;
    public int Damage;
    public float Cooldown;
    public int Upgrade;
    public bool IsLast;
    public string NextID;
    public Sprite ImageResource;
}
