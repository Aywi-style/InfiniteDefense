using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class HPRegenerationSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<HPRegeneration, HealthComponent, ViewComponent>, Exc<DeadTag, InactiveTag>> _regenerationFilter = default;

        readonly EcsPoolInject<HPRegeneration> _regenerationPool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        public void Run (EcsSystems systems)
        {
            foreach (var regenerationEntity in _regenerationFilter.Value)
            {
                ref var regenerationComponent = ref _regenerationPool.Value.Get(regenerationEntity);
                ref var healthComponent = ref _healthPool.Value.Get(regenerationEntity);
                ref var viewComponent = ref _viewPool.Value.Get(regenerationEntity);
                if (healthComponent.CurrentValue == healthComponent.MaxValue)
                {
                    continue;
                }

                if (healthComponent.CurrentValue < regenerationComponent.OldHPValue) // Если нас ударили
                {
                    regenerationComponent.OldHPValue = healthComponent.CurrentValue;
                    regenerationComponent.CurrentCooldown = regenerationComponent.MaxCooldown;
                    continue;
                }

                regenerationComponent.CurrentCooldown -= Time.deltaTime;
                if (regenerationComponent.CurrentCooldown > 0)
                {
                    continue;
                }

                if (regenerationComponent.CurrentCooldown < 0)
                {
                    regenerationComponent.CurrentCooldown = 0;
                }

                healthComponent.CurrentValue += regenerationComponent.Value * Time.deltaTime;

                if (healthComponent.CurrentValue >= healthComponent.MaxValue)
                {
                    healthComponent.CurrentValue = healthComponent.MaxValue;
                    if (viewComponent.Healthbar) viewComponent.Healthbar.ToggleSwitcher();
                }

                regenerationComponent.OldHPValue = healthComponent.CurrentValue;
                if (viewComponent.Regeneration.isStopped) viewComponent.Regeneration.Play();

                if (viewComponent.Healthbar) viewComponent.Healthbar.UpdateHealth(healthComponent.CurrentValue);
            }
        }
    }
}