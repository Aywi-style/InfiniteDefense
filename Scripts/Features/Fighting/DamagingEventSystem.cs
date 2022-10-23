using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DamagingEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsSharedInject<GameState> _state;

        readonly EcsFilterInject<Inc<DamagingEvent>> _damagingEventFilter = default;

        readonly EcsPoolInject<DamagingEvent> _damagingEventPool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<DestroyEffects> _destroyEffectsPool = default;

        readonly EcsPoolInject<TargetingEvent> _targetingEventPool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<DamagePopupEvent> _popupEvent = default;
        readonly EcsPoolInject<VibrationEvent> _vibrationPool = default;
        readonly EcsPoolInject<FiringEvent> _firingEventPool = default;
        readonly EcsPoolInject<DieEvent> _diePool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var entity in _damagingEventFilter.Value)
            {
                ref var damagingEventComponent = ref _damagingEventPool.Value.Get(entity);

                if (damagingEventComponent.TargetEntity == -1)
                {
                    Debug.Log("При проведении DamagingEvent пришла -1 энтити");
                    continue;
                }
                
                if (!_healthPool.Value.Has(damagingEventComponent.TargetEntity))
                {
                    continue;
                }

                ref var healthPointComponent = ref _healthPool.Value.Get(damagingEventComponent.TargetEntity);
                ref var viewComp = ref _viewPool.Value.Get(damagingEventComponent.TargetEntity);

                if (damagingEventComponent.DamageValue > healthPointComponent.CurrentValue)
                {
                    damagingEventComponent.DamageValue = healthPointComponent.CurrentValue;
                }

                healthPointComponent.CurrentValue -= damagingEventComponent.DamageValue;
                viewComp.Healthbar.UpdateHealth(healthPointComponent.CurrentValue);

                if (healthPointComponent.CurrentValue <= 0)
                    DieEvent(damagingEventComponent.TargetEntity);

                GameObject popup = null;
                bool popupIsOver = true;
                foreach (var item in viewComp.DamagePopups)
                {
                    if (!item.activeSelf)
                    {
                        popup = item;
                        popupIsOver = false;
                        break;
                    }
                }
                if (!popupIsOver)
                {
                    ref var popupComp = ref _popupEvent.Value.Add(_world.Value.NewEntity());
                    //popup.gameObject.transform.position = new Vector3(viewComp.GameObject.transform.position.x, viewComp.GameObject.transform.position.y + 2f, viewComp.GameObject.transform.position.z);
                    //popup.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 1);
                    popupComp.DamageAmount = (int)damagingEventComponent.DamageValue;
                    popupComp.target = new Vector3(viewComp.Transform.position.x + Random.Range(-3, 3), viewComp.Transform.position.y + Random.Range(3, 6), viewComp.Transform.position.z + Random.Range(-3, 3));
                    popupComp.DamageObject = popup;
                    popupComp.DamageObject.transform.position = new Vector3(viewComp.GameObject.transform.position.x, viewComp.GameObject.transform.position.y + 2f, viewComp.GameObject.transform.position.z);
                    popupComp.DamageObject.transform.localScale = new Vector3(0.01f, 0.01f, 1);
                    popupComp.timeOut = 1.5f;
                }

                if (damagingEventComponent.DamagingEntity == _state.Value.EntityPlayer)
                {
                    ref var vibrationEvent = ref _vibrationPool.Value.Add(entity);
                    vibrationEvent.Vibration = VibrationEvent.VibrationType.MediumImpact;
                }

                if (_targetablePool.Value.Has(damagingEventComponent.TargetEntity))
                {
                    ref var targetingEvent = ref _targetingEventPool.Value.Add(_world.Value.NewEntity());
                    targetingEvent.TargetEntity = damagingEventComponent.DamagingEntity;
                    targetingEvent.TargetingEntity = damagingEventComponent.TargetEntity;
                }

                IncreaseFireEffect(damagingEventComponent.TargetEntity);
            }
        }

        private void IncreaseFireEffect(int entity)
        {
            if (_destroyEffectsPool.Value.Has(entity) && !_firingEventPool.Value.Has(entity))
            {
               _firingEventPool.Value.Add(entity);
            }
        }
        private void DieEvent(int entity)
        {
            _diePool.Value.Add(entity);
        }
    }
}