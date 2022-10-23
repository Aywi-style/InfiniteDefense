using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public class EcsInfoMB : MonoBehaviour
    {
        private EcsWorldInject _world;

        private EcsPool<Targetable> _targetablePool;
        private EcsPool<DamagingEvent> _damagingEventPool;
        private EcsPool<DamageComponent> _damagePool;

        private EcsPool<ViewComponent> _viewPool;
        private EcsPool<Projectile> _projectilePool;
        private EcsPool<ContextToolComponent> _contextToolPool;
        private EcsPool<ActivateContextToolEvent> _activateContextToolPool;

        private EcsPool<OreEventComponent> _oreEventPool;

        [SerializeField] private int _gameObjectEntity;

        [SerializeField] private int _currentMiningOresEntity = -1;

        [SerializeField] private int _targetEntity;
        [SerializeField] private GameObject _targetObject;
        [SerializeField] private GameObject _arrowFirePoint;

        [SerializeField] private ParticleSystem OreMining;

        [SerializeField] private Transform ResHolderTransform;

        [Header("1 - Pickaxe; 2 - Sword; 3 - Bow")]
        [SerializeField] private GameObject[] _contextTools = new GameObject[3];

        public void Init(EcsWorldInject world)
        {
            _world = world;
            _targetablePool = world.Value.GetPool<Targetable>();
            _damagingEventPool = world.Value.GetPool<DamagingEvent>();
            _damagePool = world.Value.GetPool<DamageComponent>();
            _viewPool = world.Value.GetPool<ViewComponent>();
            _projectilePool = world.Value.GetPool<Projectile>();
            _contextToolPool = world.Value.GetPool<ContextToolComponent>();
            _activateContextToolPool = world.Value.GetPool<ActivateContextToolEvent>();
            _oreEventPool = world.Value.GetPool<OreEventComponent>();
        }

        public void ActivateContextTool(ContextToolComponent.Tool tool)
        {
            if (!_contextToolPool.Has(_gameObjectEntity))
            {
                return;
            }

            ref var contextToolComponent = ref _contextToolPool.Get(_gameObjectEntity);

            if (contextToolComponent.CurrentActiveTool == tool)
            {
                return;
            }

            if (!_activateContextToolPool.Has(_gameObjectEntity))
            {
                _activateContextToolPool.Add(_gameObjectEntity);
            }

            ref var activateContextToolEvent = ref _activateContextToolPool.Get(_gameObjectEntity);
            activateContextToolEvent.ActiveTool = tool;
        }

        public void DeactivateContextTool(ContextToolComponent.Tool tool)
        {
            if (!_contextToolPool.Has(_gameObjectEntity))
            {
                return;
            }

            ref var contextToolComponent = ref _contextToolPool.Get(_gameObjectEntity);

            if (contextToolComponent.CurrentActiveTool != tool)
            {
                return;
            }

            ref var activateContextToolEvent = ref _activateContextToolPool.Add(_gameObjectEntity);
            activateContextToolEvent.ActiveTool = ContextToolComponent.Tool.empty;
        }

        public int GetToolCount()
        {
            return _contextTools.Length;
        }

        public Transform GetResHolder()
        {
            return ResHolderTransform;
        }

        public void InitTools(int entity)
        {
            ref var contextToolComponent = ref _contextToolPool.Get(entity);

            for (int i = 0; i < _contextTools.Length; i++)
            {
                contextToolComponent.ToolsPool[i] = _contextTools[i];
            }
        }

        public void SetEntity(int entity)
        {
            _gameObjectEntity = entity;
        }

        public int GetEntity()
        {
            return _gameObjectEntity;
        }

        public EcsWorldInject GetWorld()
        {
            return _world;
        }

        public void SetCurrentMiningOre(int entity)
        {
            _currentMiningOresEntity = entity;
        }

        public int GetCurrentMiningOre()
        {
            return _currentMiningOresEntity;
        }

        public void SetTarget(int entity, GameObject gameObject)
        {
            if (gameObject == null) Debug.Log("Мы очищаем инфо о gameObgect нашей цели.");
            if (entity == -1) Debug.Log("Мы очищаем инфо о entity нашей цели.");
            _targetEntity = entity;
            _targetObject = gameObject;
        }
        public void SetTarget(GameObject gameObject)
        {
            if (gameObject == null)
            {
                Debug.Log("Мы не можем очистить gameObject в этом методе");
                return;
            }
            _targetObject = gameObject;
        }
        public void SetTarget(int entity)
        {
            if (entity == -1)
            {
                Debug.Log("Мы не можем очистить entity в этом методе");
                return;
            }
            _targetEntity = entity;
        }
        public void ResetTarget()
        {
            _targetEntity = -1;
            _targetObject = null;
        }

        public void MiningEvent()
        {
            var oreEntity = GetCurrentMiningOre();

            if (oreEntity > 0)
            {
                OreMining.Play();
                ref var oreEvent = ref _oreEventPool.Add(GetCurrentMiningOre());
            }
            else
            {
                Debug.Log("An error occurred during mining");
            }
        }

        public void DealDamagingEvent()
        {
            _world = GetWorld();
            _damagingEventPool = _world.Value.GetPool<DamagingEvent>();

            ref var damagingEventComponent = ref _damagingEventPool.Add(_world.Value.NewEntity());
            ref var damageComponent = ref _damagePool.Get(_gameObjectEntity);
            ref var targetableComponent = ref _targetablePool.Get(_gameObjectEntity);
            damagingEventComponent.TargetEntity = targetableComponent.TargetEntity;
            damagingEventComponent.DamageValue = damageComponent.Value;
            damagingEventComponent.DamagingEntity = _gameObjectEntity;
        }

        public void ArrowShooting()
        {
            _world = GetWorld();

            ref var targetableComponent = ref _targetablePool.Get(GetEntity());
            ref var damageComponent = ref _damagePool.Get(GetEntity());

            if (!targetableComponent.TargetObject) return;

            int arrowEntity = _world.Value.NewEntity();

            ref var arrowViewComponent = ref _viewPool.Add(arrowEntity);
            ref var projectileComponent = ref _projectilePool.Add(arrowEntity);
            ref var arrowDamageComponent = ref _damagePool.Add(arrowEntity);

            arrowViewComponent.GameObject = GameObject.Instantiate(Resources.Load<GameObject>("Arrow"), _arrowFirePoint.transform.position, Quaternion.identity);

            projectileComponent.Speed = 20;
            projectileComponent.SpeedDecreaseFactor = 1.2f;
            projectileComponent.SpeedIncreaseFactor = 0.8f;
            projectileComponent.OwnerEntity = GetEntity();
            projectileComponent.StartPosition = _arrowFirePoint.transform.position;

            projectileComponent.SupportPosition = Vector3.Lerp(gameObject.transform.position, targetableComponent.TargetObject.transform.position, 0.5f) + new Vector3(0, 5, 0);
            projectileComponent.TargetEntity = targetableComponent.TargetEntity;
            projectileComponent.TargetObject = targetableComponent.TargetObject;

            arrowDamageComponent.Value = damageComponent.Value;
        }
    }
}
