using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Collections.Generic;

namespace Client
{
    sealed class OnOffUnitAttack : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<UnitTag, Targetable>, Exc<InactiveTag, DeadTag>> _inFightFilter = default;

        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<InFightTag> _inFightPool = default;
        readonly EcsPoolInject<IsNotMovableTag> _isNotMovablePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<InMiningTag> _miningPool  = default;
        readonly EcsPoolInject<Player> _playerPool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var entity in _inFightFilter.Value)
            {
                _miningPool.Value.Del(_state.Value.EntityPlayer);
                ref var targetableComponent = ref _targetablePool.Value.Get(entity);
                ref var viewComponent = ref _viewPool.Value.Get(entity);

                bool targetObjectInDetecionZone = false;

                foreach (var entityInDetecionZone in targetableComponent.AllEntityInDetectedZone)
                {
                    if (_viewPool.Value.Get(entityInDetecionZone).GameObject == targetableComponent.TargetObject)
                    {
                        targetObjectInDetecionZone = true;
                    }
                }

                var entitysInDamageZone = new List<List<int>>();
                entitysInDamageZone.Add(targetableComponent.EntitysInMeleeZone);
                entitysInDamageZone.Add(targetableComponent.EntitysInRangeZone);

                bool targetInDamageZone = false;

                foreach (var entitysArray in entitysInDamageZone)
                {
                    foreach (var entityInDamageZone in entitysArray)
                    {
                        if (_viewPool.Value.Get(entityInDamageZone).GameObject == targetableComponent.TargetObject)
                        {
                            targetInDamageZone = true;
                        }
                    }
                }

                if (targetInDamageZone)
                {
                    if (!_isNotMovablePool.Value.Has(entity)) _isNotMovablePool.Value.Add(entity);
                }
                else
                {
                    if (_isNotMovablePool.Value.Has(entity)) _isNotMovablePool.Value.Del(entity);
                }

                if (targetObjectInDetecionZone)
                {
                    if (!_inFightPool.Value.Has(entity)) _inFightPool.Value.Add(entity);
                    viewComponent.Animator.SetBool("Attack", true);
                }
                else
                {
                    if (_inFightPool.Value.Has(entity)) _inFightPool.Value.Del(entity);
                    viewComponent.Animator.SetBool("Attack", false);
                }

                // this is very stupid kostil'. Im sry

                if (_playerPool.Value.Has(entity))
                {
                    if (targetableComponent.AllEntityInDetectedZone.Count > 0)
                    {
                        if (!_inFightPool.Value.Has(entity)) _inFightPool.Value.Add(entity);
                        viewComponent.Animator.SetBool("Attack", true);
                    }
                    else
                    {
                        if (_inFightPool.Value.Has(entity)) _inFightPool.Value.Del(entity);
                        viewComponent.Animator.SetBool("Attack", false);
                    }
                }
            }
        }
    }
}