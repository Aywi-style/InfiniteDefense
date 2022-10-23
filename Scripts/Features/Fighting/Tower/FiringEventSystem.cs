using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class FiringEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<FiringEvent>> _firingEventFilter = default;

        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<DestroyEffects> _destroyEffectsPool = default;
        readonly EcsPoolInject<FiringEvent> _firingEventPool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var eventEnetity in _firingEventFilter.Value)
            {
                ref var destroyEffectsComponent = ref _destroyEffectsPool.Value.Get(eventEnetity);
                ref var healthComponent = ref _healthPool.Value.Get(eventEnetity);

                float maxFireValue = 3;
                float fireMultiply = 1 - (healthComponent.CurrentValue / healthComponent.MaxValue);

                // 4 stages of fire: 25%, 50%, 75%, 100%

                if (fireMultiply < 0.25f) fireMultiply = 0f;
                else if (fireMultiply > 0.25f && fireMultiply < 0.5f) fireMultiply = 0.25f;
                else if (fireMultiply > 0.5f && fireMultiply < 0.75f) fireMultiply = 0.5f;
                else if (fireMultiply > 0.75f && fireMultiply < 1f) fireMultiply = 0.75f;
                else if (fireMultiply >= 1f) fireMultiply = 1f;

                if (fireMultiply != 0)
                {
                    if (destroyEffectsComponent.DestroyFire.isStopped) destroyEffectsComponent.DestroyFire.Play();
                }

                destroyEffectsComponent.DestroyFire.startSize = maxFireValue * fireMultiply;

                _firingEventPool.Value.Del(eventEnetity);
            }
        }
    }
}