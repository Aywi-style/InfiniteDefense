using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Unity;
//using Leopotam.EcsLite.UnityEditor;
using UnityEngine;

namespace Client
{
    sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField] EcsUguiEmitter _uguiEmitter;
        [SerializeField] private TowerStorage _towerStorage;
        [SerializeField] private InterfaceStorage _interfaceStorage;
        [SerializeField] private DropableItemStorage _dropableItemStorage;
        [SerializeField] private PlayerStorage _playerStorage;
        [SerializeField] private DefenseTowerStorage _defenseTowerStorage;
        [SerializeField] private EnemyConfig _enemyConfig;
        [SerializeField] private ExplosionStorage _explosionStorage;
        [SerializeField] private LevelsStorage _levelConfig;
        [Header("1 = always is MainTower")]
        [SerializeField] private int _towerCount;
        [SerializeField] private int _towersInRow;
        [SerializeField] private float _timeToNextWave;
        private WaveStorage _waveStorage;
        EcsSystems _systems;
        EcsSystems _delHereSystems;
        EcsSystems _systemsFixed;
        EcsWorld _world = null;
        GameState _gameState = null;

        void Start ()
        {
            _waveStorage = gameObject.GetComponent<WaveStorage>();
            _world = new EcsWorld();
            _gameState = new GameState(_world, _towerStorage, _interfaceStorage, _dropableItemStorage,
            _playerStorage, _defenseTowerStorage, _towerCount, _towersInRow, _timeToNextWave, _waveStorage, _enemyConfig, _explosionStorage, _levelConfig);
            _systems = new EcsSystems (_world, _gameState);
            _systemsFixed = new EcsSystems(_world, _gameState);
            _delHereSystems = new EcsSystems(_world, _gameState);
            
            _systems
                .Add(new CountdownWaveSystem())
                .Add(new WinSystem())
                //.Add(new CheckLoseSystem())
                .Add(new LoseSystem())
                .Add(new InitInterfaceSystem())
                .Add(new InitMainTower())
                .Add(new TutorialSystem())
                .Add(new StartWaveSystem())
                .Add(new StartCurrentWave())

                .Add(new InitCamera())
                .Add(new PlayerInitSystem())
                .Add(new OreInitSystem())
                
                //.Add(new RadiusInitSystem())

                .Add(new HPRegenerationSystem())
                .Add(new TowerRecoveryEventSystem())
                .Add(new CooldownSystem())

                .Add(new OnOffTowerAttack())
                
                .Add(new DefendersFallbackSystem())

                .Add(new UnitMovingSystem())
                .Add(new UnitStandingSystem())
                .Add(new DistanceToTargetSystem())
                .Add(new OnOffUnitAttack())

                .Add(new ProjectileFlyingSystem())
                .Add(new ExplosionEventSystem())

                .Add(new TargetingSystem())
                .Add(new PlayerTargetingSystem())

                .Add(new TowerLookingSystem())
                .Add(new TowerShotSystem())

                .Add(new ShipMovingSystem())
                .Add(new ShipArrivalSystem())

                .Add(new WaveSwitcher())
                .Add(new EncounterSwitcher())

                //.Add(new CheckCircleMiningSystem())
                .Add(new UserInputSystem())
                .Add(new AddCoinSystem())
                .Add(new ItemMoveToBagSystem())
                .Add(new OreMiningSystem())
                .Add(new OreMoveSystem())
                .Add(new StoneMiningSystem())
                .Add(new CameraFollowSystem())

                .Add(new UpgradeSystems())
                .Add(new UpgradeTimerSystem())
                .Add(new CreateNextTowerSystem())
                .Add(new DrawDetectionZoneEventSystem())
                .Add(new CreateNewPlayer())
                .Add(new CreateDefenders())
                .Add(new LevelPopupSystem())

                .Add(new DamagingEventSystem())
                .Add(new TargetingEventSystem())
                .Add(new DamagePopupSystem())

                .Add(new FiringEventSystem())

                .Add(new ReloadSystem())

                .Add(new DieSystem())
                .Add(new UserDropSystem())
                .Add(new DropResourcesSystem())
                .Add(new DroppedGoldSystem())

                //.Add(new DropEventSystem()) // AnyWeaponDrop
                //.Add(new PickUpNewWeaponEventSystem())

                .Add(new CorpseRemoveSystem())
                .Add(new RespawnDefender())
                .Add(new RespawnEventSystem())
                .Add(new OreRespawnSystem())
                .Add(new UpgradeCanvasSystem())
                .Add(new CanvasShipPointerSystem())
                .Add(new CanvasPointerSystem())
                .Add(new CoinPickupSystem())
                .Add(new NewTowersCircleSystem())
                .Add(new SavesSystem())
                .Add(new VibrationEventSystem())
                .Add(new CheckWinSystem())
                .Add(new KillsCounterSystem())


                .AddWorld(new EcsWorld(), Idents.Worlds.Events)
                .DelHere<ShipArrivalEvent>()
                .DelHere<ActivateWaveShipsEvent>()
                .DelHere<DamagingEvent>()
                .DelHere<TargetingEvent>()
                .DelHere<CreateNextTowerEvent>()
                ;
            _systemsFixed
                .Add(new UserMoveSystem())
                .Add(new UnitLookingSystem())
                .Add(new ActivateContextToolEventSystem())
                ;

#if UNITY_EDITOR
            _systems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
            _systemsFixed.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
                //_systems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(Idents.Worlds.Events))
                //.Add(new EcsWorldDebugSystem(Idents.Worlds.Events))
#endif         
            _systems
                .Inject()
                .InjectUgui(_uguiEmitter, Idents.Worlds.Events)
                .Init();
            _systemsFixed.Inject()
                .Init();
            _delHereSystems.Inject();
            _delHereSystems.Init();

            /*_delHereSystems
                .DelHere<StoneMiningEvent>()
                .DelHere<AddCoinEvent>()
                ;*/
        }

        void Update()
        {
            _systems?.Run();
        }
        private void FixedUpdate()
        {
            _systemsFixed?.Run();
        }

        void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems.GetWorld().Destroy();
                _systems = null;
            }
        }
    }
}