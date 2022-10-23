using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.AI;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;

namespace Client {
    sealed class CreateDefenders : IEcsRunSystem {

        readonly EcsWorldInject _world = default;

        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<CreateDefenderEvent>> _filter = default;
        readonly EcsPoolInject<MainTowerTag> _mainTowerPool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<TargetWeightComponent> _targetWeightPool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<DamageComponent> _damagePool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<DefenderComponent> _defenderPool = default;
        readonly EcsPoolInject<Resurrectable> _resurrectablePool = default;

        //todo components

        public void Run (EcsSystems systems)
        {
            foreach(var entity in _filter.Value)
            {
                ref var mainTowerComp = ref _mainTowerPool.Value.Get(_state.Value.TowersEntity[0]);
                int count = _state.Value.TowerStorage.GetDefenderCountByID(_state.Value.DefenseTowers[0]);

                for (int i = 0; i < count;i++)
                {
                    if(_state.Value.DefendersGOs[i] == null)
                    {
                        int defenderEntity = _state.Value.DefendersEntity[i];
                        ref var defenderComp = ref _defenderPool.Value.Get(defenderEntity);
                        _state.Value.DefendersGOs[i] = GameObject.Instantiate(_state.Value.TowerStorage.DefenderPrefab, defenderComp.Position, Quaternion.identity);
                        //todo заполнить энтити дефендера


                        _unitPool.Value.Add(defenderEntity);

                        ref var viewComponent = ref _viewPool.Value.Add(defenderEntity);
                        ref var healthComponent = ref _healthPool.Value.Add(defenderEntity);
                        ref var targetWeightComponent = ref _targetWeightPool.Value.Add(defenderEntity);
                        ref var movableComponent = ref _movablePool.Value.Add(defenderEntity);
                        ref var damageComponent = ref _damagePool.Value.Add(defenderEntity);
                        ref var targetableComponent = ref _targetablePool.Value.Add(defenderEntity);
                        ref var resurrectableComponent = ref _resurrectablePool.Value.Add(defenderEntity);

                        resurrectableComponent.SpawnPosition = defenderComp.Position;
                        resurrectableComponent.MaxCooldown = 5;
                        resurrectableComponent.CurrentCooldown = resurrectableComponent.MaxCooldown;

                        targetableComponent.TargetEntity = -1;
                        targetableComponent.TargetObject = null;
                        targetableComponent.AllEntityInDetectedZone = new List<int>();
                        targetableComponent.EntitysInMeleeZone = new List<int>();
                        targetableComponent.EntitysInRangeZone = new List<int>();

                        damageComponent.Value = 10;

                        movableComponent.Speed = 10f;

                        targetWeightComponent.Value = 10;

                        viewComponent.GameObject = _state.Value.DefendersGOs[i];
                        viewComponent.Transform = viewComponent.GameObject.transform;
                        viewComponent.Rigidbody = viewComponent.GameObject.GetComponent<Rigidbody>();
                        viewComponent.Animator = viewComponent.GameObject.GetComponent<Animator>();

                        viewComponent.BaseLayer = viewComponent.GameObject.layer;

                        viewComponent.EcsInfoMB = viewComponent.GameObject.GetComponent<EcsInfoMB>();
                        viewComponent.EcsInfoMB.Init(_world);
                        viewComponent.EcsInfoMB.SetEntity(defenderEntity);

                        viewComponent.AttackMB = viewComponent.GameObject.GetComponent<MeleeAttackMB>();

                        viewComponent.NavMeshAgent = viewComponent.GameObject.GetComponent<NavMeshAgent>();

                        viewComponent.Outline = viewComponent.GameObject.GetComponent<Outline>();

                        healthComponent.MaxValue = 5;
                        healthComponent.CurrentValue = healthComponent.MaxValue;

                        viewComponent.Healthbar = viewComponent.GameObject.GetComponent<HealthbarMB>();
                        viewComponent.Healthbar.SetMaxHealth(healthComponent.MaxValue);
                        viewComponent.Healthbar.SetHealth(healthComponent.MaxValue);
                        viewComponent.Healthbar.ToggleSwitcher();
                        viewComponent.Healthbar.Init(systems.GetWorld(), systems.GetShared<GameState>());

                        viewComponent.DamagePopups = new List<GameObject>();
                        for (int y = 0; y < viewComponent.Transform.GetChild(7).transform.childCount; y++)
                        {
                            var popup = viewComponent.Transform.GetChild(7).transform.GetChild(y).gameObject;
                            viewComponent.DamagePopups.Add(popup);
                            viewComponent.DamagePopups[y].SetActive(false);
                        }
                    }
                }

                _filter.Pools.Inc1.Del(entity);
            }
        }
    }
}