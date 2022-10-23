using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Client
{
    sealed class InitEnemyUnits : IEcsInitSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<EnemyTag> _enemyPool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<InactiveTag> _inactivePool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<ShipComponent> _shipPool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<DamageComponent> _damagePool = default;
        readonly EcsPoolInject<TargetWeightComponent> _targetWeightPool = default;

        EcsSharedInject<GameState> _state = default;

        private string Enemy;

        public void Init(EcsSystems systems)
        {
            var allEnemyUnits = GameObject.FindGameObjectsWithTag(nameof(Enemy));

            var world = systems.GetWorld();

            foreach (var enemy in allEnemyUnits)
            {
                var enemyEntity = world.NewEntity();

                _enemyPool.Value.Add(enemyEntity);
                _unitPool.Value.Add(enemyEntity);
                _inactivePool.Value.Add(enemyEntity);

                ref var targetableComponent = ref _targetablePool.Value.Add(enemyEntity);
                ref var movableComponent = ref _movablePool.Value.Add(enemyEntity);
                ref var viewComponent = ref _viewPool.Value.Add(enemyEntity);
                ref var shipComponent = ref _shipPool.Value.Add(enemyEntity);
                ref var healthComponent = ref _healthPool.Value.Add(enemyEntity);
                ref var damageComponent = ref _damagePool.Value.Add(enemyEntity);
                ref var targetWeightComponent = ref _targetWeightPool.Value.Add(enemyEntity);

                targetableComponent.TargetEntity = _state.Value.TowersEntity[0];
                targetableComponent.TargetObject = _viewPool.Value.Get(_state.Value.TowersEntity[0]).GameObject;

                targetableComponent.AllEntityInDetectedZone = new List<int>();
                targetableComponent.EntitysInMeleeZone = new List<int>();
                targetableComponent.EntitysInRangeZone = new List<int>();

                targetWeightComponent.Value = 5;

                healthComponent.MaxValue = 20;
                healthComponent.CurrentValue = healthComponent.MaxValue;

                damageComponent.Value = 5f;

                movableComponent.Speed = 5f;

                viewComponent.GameObject = enemy;
                viewComponent.Rigidbody = enemy.GetComponent<Rigidbody>();
                viewComponent.Animator = enemy.GetComponent<Animator>();
                viewComponent.Transform = enemy.GetComponent<Transform>();
                viewComponent.Outline = enemy.GetComponent<Outline>();
                viewComponent.AttackMB = enemy.GetComponent<MeleeAttackMB>();
                viewComponent.NavMeshAgent = enemy.GetComponent<NavMeshAgent>();
                viewComponent.EcsInfoMB = enemy.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB.Init(_world);
                viewComponent.EcsInfoMB.SetEntity(enemyEntity);
                viewComponent.EcsInfoMB.SetTarget(targetableComponent.TargetEntity, targetableComponent.TargetObject);
                viewComponent.PointerTransform = enemy.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform;
                viewComponent.Healthbar = enemy.GetComponent<HealthbarMB>();
                viewComponent.Healthbar.SetMaxHealth(healthComponent.MaxValue);
                viewComponent.Healthbar.SetHealth(healthComponent.MaxValue);
                viewComponent.Healthbar.ToggleSwitcher();
                viewComponent.Healthbar.Init(systems.GetWorld(), systems.GetShared<GameState>());
                //shipComponent.Encounter = viewComponent.GameObject.transform.parent.GetComponent<ShipArrivalMB>().GetShipEncounter();
            }
        }
    }
}