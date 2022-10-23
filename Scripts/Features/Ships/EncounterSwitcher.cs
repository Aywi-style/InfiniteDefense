using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class EncounterSwitcher : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<ShipTag, InactiveTag, CurrentWaveTag>> _InactiveShipsFilter = default;
        readonly EcsFilterInject<Inc<ShipTag, CurrentWaveTag>, Exc<InactiveTag>> _ActiveShipsFilter = default;
        readonly EcsFilterInject<Inc<EnemyTag, UnitTag>, Exc<DeadTag, InactiveTag>> _AliveUnitsFilter = default;
        readonly EcsFilterInject<Inc<EnemyTag, UnitTag, InactiveTag>, Exc<DeadTag>> _InactiveAliveUnitsFilter = default;

        readonly EcsPoolInject<ShipComponent> _shipPool = default;
        readonly EcsPoolInject<InactiveTag> _inactivePool = default;

        readonly EcsSharedInject<GameState> _state;

        private bool _encounterNeedChange = false;

        public void Run (EcsSystems systems)
        {
            if (_ActiveShipsFilter.Value.GetEntitiesCount() > 0)
            {
                return;
            }

            if (_AliveUnitsFilter.Value.GetEntitiesCount() > 0)
            {
                return;
            }

            if (_InactiveAliveUnitsFilter.Value.GetEntitiesCount() == 0)
            {
                return;
            }

            foreach (var shipEntity in _InactiveShipsFilter.Value)
            {
                ref var shipComponent = ref _shipPool.Value.Get(shipEntity);
                if (shipComponent.Encounter == _state.Value.CurrentEncounter)
                {
                    Debug.Log("Мы удалили инактив на корабле");
                    _inactivePool.Value.Del(shipEntity);
                    _encounterNeedChange = true;
                }
            }

            if (_encounterNeedChange)
            {
                _state.Value.SetNextEncounter();
                _encounterNeedChange = false;
            }

        }
    }
}