using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class TowerRecoveryEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<TowerRecoveryEvent>> _towerRecoveryEventFilter = default;

        readonly EcsPoolInject<HealthComponent> _healthComponentPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<DeadTag> _deadPool = default;
        readonly EcsPoolInject<DestroyEffects> _destroyEffectsPool = default;

        readonly EcsPoolInject<TowerRecoveryEvent> _towerRecoveryPool = default;
        readonly EcsPoolInject<FiringEvent> _firingEventPool = default;

        private float _healingPercentPerEvent = 0.05f;

        public void Run (EcsSystems systems)
        {
            foreach (var eventEntity in _towerRecoveryEventFilter.Value)
            {
                ref var towerRecoveryEvent = ref _towerRecoveryPool.Value.Get(eventEntity);

                int towerEntity = towerRecoveryEvent.TowerEntity;

                ref var healthComponent = ref _healthComponentPool.Value.Get(towerEntity);
                ref var viewComponent = ref _viewPool.Value.Get(towerEntity);

                if (healthComponent.CurrentValue >= healthComponent.MaxValue)
                {
                    _towerRecoveryPool.Value.Del(eventEntity);
                    continue;
                }

                healthComponent.CurrentValue += healthComponent.MaxValue * _healingPercentPerEvent;

                if (healthComponent.CurrentValue > healthComponent.MaxValue)
                {
                    healthComponent.CurrentValue = healthComponent.MaxValue;
                }

                viewComponent.Healthbar?.SetHealth(healthComponent.CurrentValue);
                viewComponent.Healthbar?.UpdateHealth(healthComponent.CurrentValue);

                DecreaseFireEffect(towerEntity);

                TowerResurrection(towerEntity);

                _towerRecoveryPool.Value.Del(eventEntity);
            }
        }
        private void DecreaseFireEffect(in int entity)
        {
            if (_destroyEffectsPool.Value.Has(entity))
            {
                _firingEventPool.Value.Add(entity);
            }
        }

        private void TowerResurrection(in int entity)
        {
            if (!_deadPool.Value.Has(entity))
            {
                return;
            }

            _deadPool.Value.Del(entity);

            ref var viewComponent = ref _viewPool.Value.Get(entity);

            viewComponent.GameObject.layer = viewComponent.BaseLayer;

            viewComponent.TowerWeapon?.SetActive(true);
            viewComponent.TowerFirePoint?.SetActive(true);
        }
    }
}