using UnityEngine;

namespace Client
{
    struct DetectionZone
    {
        public DetectedZoneMB DetectedZoneMB;
        public SphereCollider DetectedZoneSphere;
        public LineRenderer LineRenderer;
    }
}