using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DrawDetectionZoneEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<DrawDetectionZoneEvent>> _drawDetectionZoneEventFilter = default;

        readonly EcsPoolInject<DrawDetectionZoneEvent> _drawDetectionZoneEventPool = default;
        readonly EcsPoolInject<RadiusComponent> _radiusPool = default;
        readonly EcsPoolInject<DetectionZone> _drawingDetectionZonePool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var eventEntity in _drawDetectionZoneEventFilter.Value) // do to ay this code need to clear up
            {
                ref var radiusComponent = ref _radiusPool.Value.Get(eventEntity);
                ref var drawingDetectionZoneComponent = ref _drawingDetectionZonePool.Value.Get(eventEntity);

                float fov = 360f;
                Vector3 origin = Vector3.zero;
                int triangelesCount = 45;
                float angle = 0f;
                float angleIncrease = fov / triangelesCount;

                Vector3[] vertices = new Vector3[triangelesCount + 1 + 1];
                Vector3[] circleVerticesv = new Vector3[triangelesCount];
                drawingDetectionZoneComponent.LineRenderer.positionCount = triangelesCount;

                vertices[0] = origin;

                int vertexIndex = 1;
                int circleIndex = 0;
                for (int i = 0; i <= triangelesCount; i++)
                {
                    float angleRad = angle * (Mathf.PI / 180f);
                    Vector3 VectorFromAngle = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));

                    Vector3 vertex = origin + VectorFromAngle * radiusComponent.Radius;
                    vertices[vertexIndex] = vertex;

                    if (i > 0 && i <= circleVerticesv.Length)
                    {
                        circleVerticesv[circleIndex] = vertices[vertexIndex];
                        circleIndex++;
                    }

                    vertexIndex++;
                    angle -= angleIncrease;
                }

                drawingDetectionZoneComponent.LineRenderer.SetPositions(circleVerticesv);

                _drawDetectionZoneEventPool.Value.Del(eventEntity);
            }
        }
    }
}