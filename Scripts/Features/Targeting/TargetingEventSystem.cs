using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class TargetingEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<TargetingEvent>> _targetingEventFilter = default;

        readonly EcsPoolInject<TargetingEvent> _targetingEventPool = default;
        readonly EcsPoolInject<TargetWeightComponent> _targetWeightPool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<DeadTag> _deadPool = default;

        readonly EcsSharedInject<GameState> _state;

        public void Run (EcsSystems systems)
        {
            foreach (var entity in _targetingEventFilter.Value)
            {
                ref var targetingEvent = ref _targetingEventPool.Value.Get(entity);

                ref var targetableComponent = ref _targetablePool.Value.Get(targetingEvent.TargetingEntity);

                if (_deadPool.Value.Has(targetingEvent.TargetEntity))
                {
                    continue;
                }

                if (targetableComponent.TargetEntity == -1)
                {
                    targetableComponent.TargetEntity = targetingEvent.TargetEntity;
                    targetableComponent.TargetObject = _viewPool.Value.Get(targetingEvent.TargetEntity).GameObject;
                    _viewPool.Value.Get(targetingEvent.TargetingEntity).EcsInfoMB.SetTarget(targetableComponent.TargetEntity, targetableComponent.TargetObject);
                    continue;
                }
                
                ref var oldTargetWeightComponent = ref _targetWeightPool.Value.Get(targetableComponent.TargetEntity);
                ref var newTargetWeightComponent = ref _targetWeightPool.Value.Get(targetingEvent.TargetEntity);

                if (oldTargetWeightComponent.Value < newTargetWeightComponent.Value)
                {
                    targetableComponent.TargetEntity = targetingEvent.TargetEntity;
                    targetableComponent.TargetObject = _viewPool.Value.Get(targetingEvent.TargetEntity).GameObject;

                    _viewPool.Value.Get(targetingEvent.TargetingEntity).EcsInfoMB.SetTarget(targetableComponent.TargetEntity, targetableComponent.TargetObject);
                    continue;
                }
            }
        }
    }
}