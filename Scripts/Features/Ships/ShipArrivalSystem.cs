using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class ShipArrivalSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<ShipArrivalEvent>> _shipArrivalEventFilter = default;

        readonly EcsPoolInject<ShipArrivalEvent> _shipArrivalEventPool = default;

        readonly EcsPoolInject<ShipComponent> _shipPool = default;
        readonly EcsPoolInject<InactiveTag> _inactivePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var eventEntity in _shipArrivalEventFilter.Value)
            {
                ref var shipArrivalEvent = ref _shipArrivalEventPool.Value.Get(eventEntity);
                var shipEntity = shipArrivalEvent.ShipEntity;

                ref var shipComponent = ref _shipPool.Value.Get(shipEntity);
                _inactivePool.Value.Add(shipEntity);

                foreach (var enemyEntity in shipComponent.EnemyUnitsEntitys)
                {
                    ref var viewComponent = ref _viewPool.Value.Get(enemyEntity);
                    viewComponent.GameObject.transform.SetParent(null);

                    _inactivePool.Value.Del(enemyEntity);
                }
            }
        }
    }
}