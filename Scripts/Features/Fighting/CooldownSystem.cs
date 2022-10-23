using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CooldownSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Cooldown>> _cooldownFilter = default;

        readonly EcsPoolInject<Cooldown> _cooldownPool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var cooldownEntity in _cooldownFilter.Value)
            {
                ref var cooldownComponent = ref _cooldownPool.Value.Get(cooldownEntity);

                if (cooldownComponent.CurrentValue == 0)
                {
                    continue;
                }

                cooldownComponent.CurrentValue -= Time.deltaTime;

                if (cooldownComponent.CurrentValue < 0)
                {
                    cooldownComponent.CurrentValue = 0;
                }
            }
        }
    }
}