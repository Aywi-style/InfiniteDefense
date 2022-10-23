using UnityEngine;

namespace Client
{
    struct Projectile
    {
        public int OwnerEntity;
        public float Speed;
        public float SpeedDecreaseFactor;
        public float SpeedIncreaseFactor;
        public int TargetEntity;
        public Vector3 StartPosition;
        public Vector3 SupportPosition;
        public GameObject TargetObject;

        public bool IsExploding;
        public ExplosionEvent.Size ExplosionValue;
    }
}