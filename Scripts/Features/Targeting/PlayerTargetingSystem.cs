using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Collections.Generic;

namespace Client
{
    sealed class PlayerTargetingSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Targetable, Player, InFightTag>, Exc<DeadTag>> _playerFilter = default;

        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<DeadTag> _deadPool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var entity in _playerFilter.Value)
            {
                ref var targetableComponent = ref _targetablePool.Value.Get(entity);
                ref var viewComponent = ref _viewPool.Value.Get(entity);

                if (targetableComponent.TargetEntity > 0)
                {
                    if (_deadPool.Value.Has(targetableComponent.TargetEntity))
                    {
                        ResetTarget(ref targetableComponent, ref viewComponent);
                    }
                }

                if (targetableComponent.TargetEntity > -1 && targetableComponent.TargetObject == null)
                {
                    targetableComponent.TargetObject = _viewPool.Value.Get(targetableComponent.TargetEntity).GameObject;
                }

                if (targetableComponent.AllEntityInDetectedZone.Count == 0)
                {
                    ResetTarget(ref targetableComponent, ref viewComponent);
                    continue;
                }

                if (targetableComponent.EntitysInMeleeZone.Count > 0)
                {
                    if (targetableComponent.TargetEntity != targetableComponent.EntitysInMeleeZone[0])
                    {
                        targetableComponent.TargetEntity = targetableComponent.EntitysInMeleeZone[0];
                        targetableComponent.TargetObject = _viewPool.Value.Get(targetableComponent.TargetEntity).GameObject;
                        viewComponent.EcsInfoMB.SetTarget(targetableComponent.TargetEntity, targetableComponent.TargetObject);
                    }
                }

                var allDeadEntitys = new List<int>();

                var DamageZones = new List<List<int>>();
                DamageZones.Add(targetableComponent.EntitysInMeleeZone);
                DamageZones.Add(targetableComponent.EntitysInRangeZone);

                bool targetInAnyDamageZone = false;

                foreach (var zoneType in DamageZones)
                {
                    foreach (var entityInDamageZone in zoneType)
                    {
                        if (_deadPool.Value.Has(entityInDamageZone))
                        {
                            allDeadEntitys.Add(entityInDamageZone);
                            Debug.Log("Энтити находилась в пуле мертвых");
                        }

                        if (_viewPool.Value.Get(entityInDamageZone).GameObject == targetableComponent.TargetObject)
                        {
                            targetInAnyDamageZone = true;
                        }
                    }
                }

                foreach (var entityInDamageZone in targetableComponent.EntitysInMeleeZone)
                {
                    if (_deadPool.Value.Has(entityInDamageZone))
                    {
                        allDeadEntitys.Add(entityInDamageZone);
                        Debug.Log("Энтити находилась в пуле мертвых");
                    }
                    else if (_viewPool.Value.Get(entityInDamageZone).GameObject == targetableComponent.TargetObject)
                    {
                        targetInAnyDamageZone = true;
                    }
                }

                foreach (var deadEntity in allDeadEntitys)
                {
                    targetableComponent.EntitysInMeleeZone.Remove(deadEntity);
                    targetableComponent.EntitysInRangeZone.Remove(deadEntity);
                }

                if (targetableComponent.EntitysInRangeZone.Count == 0 && targetableComponent.EntitysInMeleeZone.Count == 0)
                {
                    ResetTarget(ref targetableComponent, ref viewComponent);
                    continue;
                }

                if (!targetInAnyDamageZone)
                {
                    if (targetableComponent.EntitysInMeleeZone.Count > 0)
                    {
                        targetableComponent.TargetEntity = targetableComponent.EntitysInMeleeZone[0];
                    }
                    else
                    {
                        targetableComponent.TargetEntity = targetableComponent.EntitysInRangeZone[0];
                    }
                    
                    targetableComponent.TargetObject = _viewPool.Value.Get(targetableComponent.TargetEntity).GameObject;
                    viewComponent.EcsInfoMB.SetTarget(targetableComponent.TargetEntity, targetableComponent.TargetObject);
                }

                if (targetableComponent.TargetEntity < 1)
                {
                    if (targetableComponent.EntitysInMeleeZone.Count > 0)
                    {
                        targetableComponent.TargetEntity = targetableComponent.EntitysInMeleeZone[0];
                    }
                    else
                    {
                        targetableComponent.TargetEntity = targetableComponent.EntitysInRangeZone[0];
                    }

                    targetableComponent.TargetObject = _viewPool.Value.Get(targetableComponent.TargetEntity).GameObject;
                    viewComponent.EcsInfoMB.SetTarget(targetableComponent.TargetEntity, targetableComponent.TargetObject);
                }
            }
        }

        private void ResetTarget(ref Targetable targetableComponent, ref ViewComponent viewComponent)
        {
            targetableComponent.TargetEntity = -1;
            targetableComponent.TargetObject = null;
            viewComponent.EcsInfoMB.ResetTarget();
        }
    }
}