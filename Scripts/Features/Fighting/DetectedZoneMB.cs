using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public class DetectedZoneMB : MonoBehaviour
    {
        [SerializeField] private GameObject _mainGameObject;
        [SerializeField] private EcsInfoMB _ecsInfoMB;
        [SerializeField] private Material SleepingMaterial;
        [SerializeField] private Material AwakenedMaterial;
        [SerializeField] private MeshRenderer MeshRenderer;

        private EcsWorldInject _world;

        private EcsPool<Targetable> _targetablePool;
        private EcsPool<DeadTag> _deadPool;

        private string _enemyTag = "Enemy";
        private string _friendlyTag = "Friendly";
        private string _targetTag;

        public void Start()
        {
            if (_mainGameObject == null) _mainGameObject = transform.parent.gameObject;
            if (_ecsInfoMB == null) _ecsInfoMB = GetComponentInParent<EcsInfoMB>();

            if (SleepingMaterial && AwakenedMaterial)
            {
                MeshRenderer = _mainGameObject.GetComponent<MeshRenderer>();
            }

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

            if (_ecsInfoMB.GetWorld().Value.GetPool<DeadTag>().Has(other.GetComponent<EcsInfoMB>().GetEntity()))
            {
                return;
            }

            _world = _ecsInfoMB.GetWorld();
            _targetablePool = _world.Value.GetPool<Targetable>();
            ref var targetableComponent = ref _targetablePool.Get(_ecsInfoMB.GetEntity());
            targetableComponent.AllEntityInDetectedZone.Add(other.GetComponent<EcsInfoMB>().GetEntity());

            if (MeshRenderer != null)
            {
                MeshRenderer.material = AwakenedMaterial;
            }
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

            /*if (_ecsInfoMB.GetWorld().Value.GetPool<DeadTag>().Has(other.GetComponent<EcsInfoMB>().GetEntity()))
            {
                Debug.Log("Этот чел уже мёртв, сорянба");
                return;
            }*/

            _world = _ecsInfoMB.GetWorld();
            _targetablePool = _world.Value.GetPool<Targetable>();
            ref var targetableComponent = ref _targetablePool.Get(_ecsInfoMB.GetEntity());
            targetableComponent.AllEntityInDetectedZone.Remove(other.GetComponent<EcsInfoMB>().GetEntity());

            if (MeshRenderer != null && targetableComponent.AllEntityInDetectedZone.Count < 1)
            {
                MeshRenderer.material = SleepingMaterial;
            }
        }
    }
}
