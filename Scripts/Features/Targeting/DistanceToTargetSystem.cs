using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DistanceToTargetSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Targetable, ViewComponent>, Exc<InactiveTag>> _entitysFilter = default;

        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        public void Run (EcsSystems systems)
        {
            foreach(var entity in _entitysFilter.Value)
            {
                ref var targetableComponent = ref _targetablePool.Value.Get(entity);
                if (targetableComponent.TargetObject == null)
                {
                    continue;
                }

                ref var viewComponent = ref _viewPool.Value.Get(entity);
                targetableComponent.DistanceToTarget = Mathf.Sqrt((targetableComponent.TargetObject.transform.position - viewComponent.GameObject.transform.position).sqrMagnitude);
            }
        }
    }
}