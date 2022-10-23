using UnityEngine;

namespace Client
{
    public struct ExplosionEvent
    {
        public enum Size
        {
            Small,
            Medium,
            Large
        }

        public Size Value;
        public Vector3 Point;
    }
}