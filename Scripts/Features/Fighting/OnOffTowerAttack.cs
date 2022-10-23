using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class OnOffTowerAttack : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<TowerTag, Targetable>, Exc<InactiveTag, DeadTag, UnitTag>> _towerFilter = default;

        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<InFightTag> _inFightPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<DetectionZone> _drawingDetectionZonePool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var towerEntity in _towerFilter.Value)
            {
                ref var targetableComponent = ref _targetablePool.Value.Get(towerEntity);
                ref var viewComponent = ref _viewPool.Value.Get(towerEntity);
                ref var drawingDetectionZone = ref _drawingDetectionZonePool.Value.Get(towerEntity);

                if (targetableComponent.EntitysInRangeZone.Count == 0)
                {
                    if (_inFightPool.Value.Has(towerEntity)) _inFightPool.Value.Del(towerEntity);
                    drawingDetectionZone.LineRenderer.enabled = false;
                    //здесь башня должна уйти в своё стандартное положение
                }
                else
                {
                    if (!_inFightPool.Value.Has(towerEntity)) _inFightPool.Value.Add(towerEntity);
                    drawingDetectionZone.LineRenderer.enabled = true;
                }
            }
        }
    }
}