using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class WaveSwitcher : IEcsRunSystem
    {

        readonly EcsFilterInject<Inc<EnemyTag, ShipTag, ShipComponent>> _shipsFilter = default;

        readonly EcsPoolInject<ShipComponent> _shipPool = default;
        readonly EcsPoolInject<CurrentWaveTag> _currentWavePool = default;

        readonly EcsSharedInject<GameState> _state;

        public void Run (EcsSystems systems)
        {
            foreach (var shipEntity in _shipsFilter.Value)
            {
                ref var shipComponent = ref _shipPool.Value.Get(shipEntity);

                if (_currentWavePool.Value.Has(shipEntity))
                {
                    _currentWavePool.Value.Del(shipEntity);
                }

                if (shipComponent.Wave == _state.Value.GetCurrentWave())
                {
                    _currentWavePool.Value.Add(shipEntity);
                }
            }
        }
    }
}