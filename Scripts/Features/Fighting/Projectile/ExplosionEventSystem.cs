using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ExplosionEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsSharedInject<GameState> _state = default;

        readonly EcsFilterInject<Inc<ExplosionEvent>> _explosionEventFilter = default;

        readonly EcsPoolInject<ExplosionEvent> _explosionEventPool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var eventEntity in _explosionEventFilter.Value)
            {
                ref var explosionEventComponent = ref _explosionEventPool.Value.Get(eventEntity);

                int explosionSize = (int)explosionEventComponent.Value;

                var explosion = GameObject.Instantiate(_state.Value.ExplosionStorage.ExplosionPrefab[explosionSize], explosionEventComponent.Point, Quaternion.identity);

                explosion.GetComponent<ParticleSystem>().Play();

                _world.Value.DelEntity(eventEntity);
            }
        }
    }
}