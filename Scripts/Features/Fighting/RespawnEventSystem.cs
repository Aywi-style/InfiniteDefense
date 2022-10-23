using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class RespawnEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Resurrectable, RespawnEvent, DeadTag>> _respawnEventFilter = default;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<Resurrectable> _resurrectablePool = default;
        readonly EcsPoolInject<RespawnEvent> _respawnPool = default;
        readonly EcsPoolInject<DeadTag> _deadPool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var respawningEntity in _respawnEventFilter.Value)
            {
                ref var resurrectableComponent = ref _resurrectablePool.Value.Get(respawningEntity);

                if (resurrectableComponent.CurrentCooldown > 0)
                {
                    resurrectableComponent.CurrentCooldown -= Time.deltaTime;
                    continue;
                }

                resurrectableComponent.CurrentCooldown = resurrectableComponent.MaxCooldown;

                ref var healthComponent = ref _healthPool.Value.Get(respawningEntity);
                ref var viewComponent = ref _viewPool.Value.Get(respawningEntity);

                if (resurrectableComponent.OnSpawnPosition) viewComponent.GameObject.transform.position = resurrectableComponent.SpawnPosition;

                healthComponent.CurrentValue = healthComponent.MaxValue;

                if (viewComponent.GameObject.layer == LayerMask.NameToLayer("Dead")) viewComponent.GameObject.layer = viewComponent.BaseLayer;

                if (viewComponent.Animator) viewComponent.Animator.SetTrigger("Resurrection");
                if (viewComponent.Outline) viewComponent.Outline.enabled = true;
                if (viewComponent.NavMeshAgent) viewComponent.NavMeshAgent.enabled = true;
                if (viewComponent.Healthbar) viewComponent.Healthbar.Enableble();
                _deadPool.Value.Del(respawningEntity);
                _respawnPool.Value.Del(respawningEntity);
            }
        }
    }
}