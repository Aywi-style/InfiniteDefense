using UnityEngine;

namespace Client
{
    struct Resurrectable
    {
        public Vector3 SpawnPosition;
        public float MaxCooldown;
        public float CurrentCooldown;
        public bool OnSpawnPosition;
    }
}