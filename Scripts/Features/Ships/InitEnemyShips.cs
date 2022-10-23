using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitEnemyShips : IEcsInitSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsPoolInject<InactiveTag> _inactivePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        readonly EcsSharedInject<GameState> _state;

        public void Init (EcsSystems systems)
        {
            //var allShips = GameObject.FindGameObjectsWithTag(nameof(Ship));
            var allShips = GameObject.FindObjectsOfType<ShipArrivalMB>();

            foreach (var ship in allShips)
            {
                var shipEntity = _world.Value.NewEntity();

                _world.Value.GetPool<EnemyTag>().Add(shipEntity);

                _world.Value.GetPool<ShipTag>().Add(shipEntity);

                ref var targetableComponent = ref _world.Value.GetPool<Targetable>().Add(shipEntity);

                ref var _viewMainTowerComponent = ref _viewPool.Value.Get(_state.Value.TowersEntity[0]);

                targetableComponent.TargetEntity = _state.Value.TowersEntity[0];
                targetableComponent.TargetObject = _viewMainTowerComponent.GameObject;

                ref var movableComponent = ref _world.Value.GetPool<Movable>().Add(shipEntity);
                movableComponent.Speed = 10f;

                // ref var shipComponent = ref _shipPool.Value.Add(shipEntity);
                // shipComponent.ShipArrivalMB = ship.GetComponent<ShipArrivalMB>();
                // shipComponent.ShipArrivalMB.SetEntity(shipEntity);
                // shipComponent.ShipArrivalMB.Init(_world);
                // shipComponent.Encounter = shipComponent.ShipArrivalMB.GetShipEncounter();
                // shipComponent.Wave = shipComponent.ShipArrivalMB.GetShipWave();
                // shipComponent.EnemyUnitsEntitys = new List<int>();

                ref var viewComponent = ref _viewPool.Value.Add(shipEntity);
                viewComponent.GameObject = ship.gameObject;
                viewComponent.Rigidbody = ship.GetComponent<Rigidbody>();

                _inactivePool.Value.Add(shipEntity);

                // foreach (var enemyUnit in _enemyUnitsFilter.Value)
                // {
                //     ref var enemyUnitShip = ref _shipPool.Value.Get(enemyUnit);
                //     ref var enemyViewComponent = ref _viewPool.Value.Get(enemyUnit);

                //     if (viewComponent.GameObject == enemyViewComponent.GameObject.transform.parent.gameObject)
                //     {
                //         shipComponent.EnemyUnitsEntitys.Add(enemyUnit);
                //     }
                // }
            }
        }
    }
}