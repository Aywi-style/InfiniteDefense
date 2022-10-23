using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public class ShipArrivalMB : MonoBehaviour
    {
        [SerializeField] private EcsInfoMB _ecsInfoMB;

        private EcsWorldInject _world;

        private EcsPool<ShipArrivalEvent> _shipArrivalEventPool;
        private EcsPool<InactiveTag> _inactivePool;

        void Start()
        {
            if (_ecsInfoMB == null) _ecsInfoMB = gameObject.GetComponent<EcsInfoMB>();
        }
        private void OnTriggerEnter(Collider land)
        {
            _inactivePool = _ecsInfoMB.GetWorld().Value.GetPool<InactiveTag>();
            if (_inactivePool.Has(_ecsInfoMB.GetEntity()))
            {
                return;
            }

            if (land.CompareTag("LandTrigger"))
            {
                _world = _ecsInfoMB.GetWorld();
                _shipArrivalEventPool = _world.Value.GetPool<ShipArrivalEvent>();
                ref var shipArrivalEvent = ref _shipArrivalEventPool.Add(_world.Value.NewEntity());
                shipArrivalEvent.ShipEntity = _ecsInfoMB.GetEntity();
                
            }

        }
    }
}
