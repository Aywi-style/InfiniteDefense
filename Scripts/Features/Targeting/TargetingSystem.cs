using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// �������� � ������ � ���� �����������
    /// </summary>
    sealed class TargetingSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Targetable>, Exc<InactiveTag, DeadTag, Projectile>> _targetableFilter = default;

        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<DeadTag> _deadPool = default;
        readonly EcsPoolInject<EnemyTag> _enemyPool = default;
        readonly EcsPoolInject<ShipTag> _shipPool = default;

        readonly EcsSharedInject<GameState> _state;

        public void Run (EcsSystems systems) // to do ay if unit is not in fight do retarget on any object in hes DetectedZone
        {
            foreach(var entity in _targetableFilter.Value)
            {
                ref var targetableComponent = ref _targetablePool.Value.Get(entity);
                ref var viewComponent = ref _viewPool.Value.Get(entity);

                if (targetableComponent.TargetEntity > -1)
                {
                    if (targetableComponent.TargetObject == null)
                    {
                        targetableComponent.TargetObject = _viewPool.Value.Get(targetableComponent.TargetEntity).GameObject;
                    }

                    if (_deadPool.Value.Has(targetableComponent.TargetEntity))
                    {
                        targetableComponent.TargetEntity = -1;
                        targetableComponent.TargetObject = null;
                        viewComponent.EcsInfoMB.ResetTarget();
                    }
                }

                if (_shipPool.Value.Has(entity))
                {
                    continue;
                }

                if (targetableComponent.AllEntityInDetectedZone.Count == 0 && targetableComponent.TargetEntity == -1)
                {
                    if (_enemyPool.Value.Has(entity))
                    {
                        targetableComponent.TargetEntity = _state.Value.TowersEntity[0];
                        targetableComponent.TargetObject = _viewPool.Value.Get(_state.Value.TowersEntity[0]).GameObject;
                        viewComponent.EcsInfoMB.SetTarget(targetableComponent.TargetEntity, targetableComponent.TargetObject);
                    }
                    else
                    {
                        targetableComponent.TargetEntity = -1;
                        targetableComponent.TargetObject = null;
                        viewComponent.EcsInfoMB.ResetTarget();
                    }
                    continue;
                }

                List<int> allDeadEntitys = new List<int>();

                foreach (var entityInDetectedZone in targetableComponent.AllEntityInDetectedZone)
                {
                    if (_deadPool.Value.Has(entityInDetectedZone))
                    {
                        allDeadEntitys.Add(entityInDetectedZone);
                    }
                }

                foreach (var deadEntity in allDeadEntitys)
                {
                    targetableComponent.AllEntityInDetectedZone.Remove(deadEntity);
                }

                if (targetableComponent.AllEntityInDetectedZone.Count == 0)
                {
                    continue;
                }

                if (targetableComponent.TargetEntity < 1 || targetableComponent.TargetEntity == _state.Value.EntityMainTower)
                {
                    targetableComponent.TargetEntity = targetableComponent.AllEntityInDetectedZone[0];
                    targetableComponent.TargetObject = _viewPool.Value.Get(targetableComponent.TargetEntity).GameObject;
                    viewComponent.EcsInfoMB.SetTarget(targetableComponent.TargetEntity, targetableComponent.TargetObject);
                }
            }
        }
    }
}