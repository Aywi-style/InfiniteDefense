using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class UnitStandingSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<UnitTag, ViewComponent, Movable, IsNotMovableTag>, Exc<Player, DeadTag, InactiveTag>> _allUnitsFilter = default;

        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (var unitEntity in _allUnitsFilter.Value)
            {
                ref var movableComponent = ref _movablePool.Value.Get(unitEntity);

                ref var viewComponent = ref _viewPool.Value.Get(unitEntity);

                viewComponent.Animator.SetBool("Run", false);
                viewComponent.Rigidbody.velocity = Vector3.zero;
                viewComponent.NavMeshAgent.ResetPath();
            }
        }
    }
}