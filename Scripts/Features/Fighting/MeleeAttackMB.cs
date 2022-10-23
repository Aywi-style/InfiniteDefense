using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public class MeleeAttackMB : MonoBehaviour
    {
        [SerializeField] private GameObject _mainGameObject;
        [SerializeField] private EcsInfoMB _ecsInfoMB;
        [SerializeField] private Animator _animator;

        private EcsWorldInject _world;

        private EcsPool<Targetable> _targetablePool;

        private string _enemyTag = "Enemy";
        private string _friendlyTag = "Friendly";
        private string _targetTag;

        private int _enemyInZone;

        private ContextToolComponent.Tool _thisTool = ContextToolComponent.Tool.sword;

        void Start()
        {
            if (_mainGameObject == null) _mainGameObject = transform.parent.gameObject;
            if (_ecsInfoMB == null) _ecsInfoMB = _mainGameObject.GetComponent<EcsInfoMB>();
            if (_animator == null) _animator = _mainGameObject.GetComponent<Animator>();

            _enemyInZone = 0;

            if (_mainGameObject.CompareTag(_enemyTag))
            {
                _targetTag = _friendlyTag;
            }
            else
            {
                _targetTag = _enemyTag;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
            {
                return;
            }

            if (!other.gameObject.CompareTag(_targetTag))
            {
                return;
            }

            _animator.SetLayerWeight(1, 1);
            _animator.SetBool("Melee", true);

            _world = _ecsInfoMB.GetWorld();
            var thisObjectEntity = _ecsInfoMB.GetEntity();
            _targetablePool = _world.Value.GetPool<Targetable>();

            ref var targetableComponent = ref _targetablePool.Get(thisObjectEntity);
            targetableComponent.EntitysInMeleeZone.Add(other.GetComponent<EcsInfoMB>().GetEntity());
            _enemyInZone++;

            _ecsInfoMB.ActivateContextTool(_thisTool);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger)
            {
                return;
            }

            if (!other.gameObject.CompareTag(_targetTag))
            {
                return;
            }

            _world = _ecsInfoMB.GetWorld();
            _targetablePool = _world.Value.GetPool<Targetable>();
            ref var targetableComponent = ref _targetablePool.Get(_ecsInfoMB.GetEntity());
            targetableComponent.EntitysInMeleeZone.Remove(other.GetComponent<EcsInfoMB>().GetEntity());

            _enemyInZone--;

            if (_enemyInZone <= 0)
            {
                _animator.SetBool("Melee", false);
            }

            if (targetableComponent.EntitysInMeleeZone.Count > 0)
            {
                _ecsInfoMB.ActivateContextTool(ContextToolComponent.Tool.bow); // rewrite if will be more long-range weapons
                return;
            }

            _ecsInfoMB.DeactivateContextTool(_thisTool);
        }
    }
}
