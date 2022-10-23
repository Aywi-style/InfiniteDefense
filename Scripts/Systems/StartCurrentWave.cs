using Leopotam.EcsLite;
using UnityEngine;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Client {
    sealed class StartCurrentWave : IEcsRunSystem {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<StartWaveEvent>> _filter = default;
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
        readonly EcsPoolInject<DropableItem> _dropableItemPool = default;
        readonly EcsFilterInject<Inc<TutorialComponent>> _tutorialPool = default;
        private float _angle = 0f;
        private float _shipAngle = 0f;
        private int _encounter = 0;
        private int _enemyCountInEncounter = 0;
        private int _enemySpawnRadius = 90;

        public void Run (EcsSystems systems) {
            foreach(var entity in _filter.Value)
            {
                ref var filterComp = ref _filter.Pools.Inc1.Get(entity);

                for (int wave = 0; wave < _state.Value.WaveStorage.Waves.Count; wave++)
                {
                    int[] enemyInShip = _state.Value.WaveStorage.Waves[wave].MeleeEnemyInShip;
                    int[] rangeEnemyInShip = _state.Value.WaveStorage.Waves[wave].RangeEnemyInShip;
                    int[] encounters = _state.Value.WaveStorage.Waves[wave].Encounters;

                    for (int i = 0; i < enemyInShip.Length; i++)
                    {
                        _angle += Random.Range(30,360 / enemyInShip.Length + 20);

                        var x = Mathf.Cos(_angle * Mathf.Deg2Rad) * _enemySpawnRadius;
                        var z = Mathf.Sin(_angle * Mathf.Deg2Rad) * _enemySpawnRadius;
                        var ship = GameObject.Instantiate(_state.Value.EnemyConfig.ShipPrefab, new Vector3(x, 0, z), Quaternion.identity);
                        

                        var shipEntity = _world.Value.NewEntity();

                        _world.Value.GetPool<EnemyTag>().Add(shipEntity);

                        _world.Value.GetPool<ShipTag>().Add(shipEntity);

                        ref var targetableComponent = ref _world.Value.GetPool<Targetable>().Add(shipEntity);

                        ref var _viewMainTowerComponent = ref _viewPool.Value.Get(_state.Value.TowersEntity[0]);

                        targetableComponent.TargetEntity = _state.Value.TowersEntity[0];
                        targetableComponent.TargetObject = _viewMainTowerComponent.GameObject;

                        ref var movableComponent = ref _world.Value.GetPool<Movable>().Add(shipEntity);
                        movableComponent.Speed = 8f;

                        ref var shipComponent = ref _shipPool.Value.Add(shipEntity);
                        shipComponent.Encounter = _encounter;
                        shipComponent.Wave = wave;
                        shipComponent.EnemyUnitsEntitys = new List<int>();

                        ref var viewComponent = ref _viewPool.Value.Add(shipEntity);
                        viewComponent.GameObject = ship.gameObject;
                        viewComponent.Rigidbody = ship.GetComponent<Rigidbody>();
                        viewComponent.EcsInfoMB = ship.GetComponent<EcsInfoMB>();
                        viewComponent.EcsInfoMB.Init(_world);
                        viewComponent.EcsInfoMB.SetEntity(shipEntity);

                        if (!ship.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform) Debug.LogError("На корабле нет CanvasPointer'a");
                        viewComponent.PointerTransform = ship.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform;

                        _inactivePool.Value.Add(shipEntity);

                        string[] enemies = new string[enemyInShip[i] + rangeEnemyInShip[i]];

                        for (int n = 0; n < enemies.Length; n++)
                        {
                            if (n < enemyInShip[i])
                            {
                                enemies[n] = "Melee";
                            }
                            else
                            {
                                enemies[n] = "Range";
                            }
                        }

                        for (int j = 0; j < enemies.Length; j++)
                        {
                            var ex = Mathf.Cos(_shipAngle * Mathf.Deg2Rad) * 2;
                            var ez = Mathf.Sin(_shipAngle * Mathf.Deg2Rad) * 2;
                            if (enemyInShip[i] == 0)
                            {
                                _shipAngle += 360;
                            }
                            else
                            {
                                _shipAngle += 360 / enemyInShip[i];
                            }

                            GameObject enemy = null;

                            var enemyEntity = _world.Value.NewEntity();

                            _enemyPool.Value.Add(enemyEntity);
                            _unitPool.Value.Add(enemyEntity);
                            _inactivePool.Value.Add(enemyEntity);

                            ref var unitTargetableComponent = ref _targetablePool.Value.Add(enemyEntity);
                            ref var unitMovableComponent = ref _movablePool.Value.Add(enemyEntity);
                            ref var unitViewComponent = ref _viewPool.Value.Add(enemyEntity);
                            ref var unitShipComponent = ref _shipPool.Value.Add(enemyEntity);
                            ref var unitHealthComponent = ref _healthPool.Value.Add(enemyEntity);
                            ref var unitDamageComponent = ref _damagePool.Value.Add(enemyEntity);
                            ref var unitTargetWeightComponent = ref _targetWeightPool.Value.Add(enemyEntity);

                            switch (enemies[j])
                            {
                                case "Melee":
                                    {
                                        enemy = GameObject.Instantiate(_state.Value.EnemyConfig.EnemyPrefab, ship.transform);
                                        unitViewComponent.GameObject = enemy;
                                        unitViewComponent.Animator = enemy.GetComponent<Animator>();
                                        
                                        unitDamageComponent.Value = 10f;

                                        unitHealthComponent.MaxValue = 50;

                                        unitTargetWeightComponent.Value = 3;

                                        break;
                                    }
                                case "Range":
                                    {
                                        enemy = GameObject.Instantiate(_state.Value.EnemyConfig.RangeEnemyPrefab, ship.transform);
                                        unitViewComponent.GameObject = enemy;
                                        unitViewComponent.Animator = enemy.GetComponent<Animator>();

                                        unitDamageComponent.Value = 7f;

                                        unitHealthComponent.MaxValue = 30;

                                        unitTargetWeightComponent.Value = 5;

                                        break;
                                    }
                            }

                            enemy.transform.localPosition = new Vector3(ex, 0, ez);

                            unitTargetableComponent.TargetEntity = _state.Value.EntityMainTower;
                            unitTargetableComponent.TargetObject = _viewPool.Value.Get(_state.Value.TowersEntity[0]).GameObject;

                            unitTargetableComponent.AllEntityInDetectedZone = new List<int>();
                            unitTargetableComponent.EntitysInMeleeZone = new List<int>();
                            unitTargetableComponent.EntitysInRangeZone = new List<int>();

                            unitHealthComponent.CurrentValue = unitHealthComponent.MaxValue;

                            unitMovableComponent.Speed = 5f;

                            unitViewComponent.Rigidbody = enemy.GetComponent<Rigidbody>();
                            unitViewComponent.Transform = enemy.GetComponent<Transform>();
                            unitViewComponent.Outline = enemy.GetComponent<Outline>();
                            unitViewComponent.AttackMB = enemy.GetComponent<MeleeAttackMB>();
                            unitViewComponent.NavMeshAgent = enemy.GetComponent<NavMeshAgent>();
                            unitViewComponent.NavMeshAgent.speed = unitMovableComponent.Speed;
                            unitViewComponent.BodyCollider = enemy.GetComponent<CapsuleCollider>();
                            unitViewComponent.EcsInfoMB = enemy.GetComponent<EcsInfoMB>();
                            unitViewComponent.EcsInfoMB.Init(_world);
                            unitViewComponent.EcsInfoMB.SetEntity(enemyEntity);
                            unitViewComponent.EcsInfoMB.SetTarget(unitTargetableComponent.TargetEntity, unitTargetableComponent.TargetObject);
                            unitViewComponent.PointerTransform = enemy.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform;
                            unitViewComponent.Healthbar = enemy.GetComponent<HealthbarMB>();
                            unitViewComponent.Healthbar.SetMaxHealth(unitHealthComponent.MaxValue);
                            unitViewComponent.Healthbar.SetHealth(unitHealthComponent.MaxValue);
                            unitViewComponent.Healthbar.ToggleSwitcher();
                            unitViewComponent.Healthbar.Init(systems.GetWorld(), systems.GetShared<GameState>());
                            unitViewComponent.DamagePopups = new List<GameObject>();

                            /*foreach (var item in _tutorialPool.Value)
                            {
                                if (_tutorialPool.Pools.Inc1.Get(item).TutorialStage == 11)
                                {
                                    _state.Value.Saves.TutorialStage = 12;
                                    _state.Value.Saves.SaveTutorial(12);
                                    _tutorialPool.Pools.Inc1.Get(item).TutorialStage = 12;
                                }
                            }*/

                            for (int y = 0; y < unitViewComponent.Transform.GetChild(0).transform.childCount; y++)
                            {
                                var popup = unitViewComponent.Transform.GetChild(0).transform.GetChild(y).gameObject;
                                unitViewComponent.DamagePopups.Add(popup);
                                unitViewComponent.DamagePopups[y].SetActive(false);
                            }
                            
                            unitShipComponent.Encounter = _encounter;

                            shipComponent.EnemyUnitsEntitys.Add(enemyEntity);


                        }
                        _enemyCountInEncounter++;
                        //Debug.Log("Dlinna " + encounters.Length + " " + _encounter + " " + encounters[_encounter] + " " + _enemyCountInEncounter);
                        if (_encounter <= encounters.Length && _enemyCountInEncounter == encounters[_encounter])
                        {
                            _encounter++;
                            _enemyCountInEncounter = 0;
                        }

                        _shipAngle = 0f;
                    }
                    _angle = 0f;
                    _encounter = 0;
                    _enemyCountInEncounter = 0;
                }
                // _state.Value.Saves.CurrentWave++;
                // _state.Value.Saves.SaveCurrentWave(_state.Value.Saves.CurrentWave);
                
                _filter.Pools.Inc1.Del(entity);
            }
        }
    }
}