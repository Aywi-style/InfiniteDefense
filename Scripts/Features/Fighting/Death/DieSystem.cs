using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DieSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world = default;
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<HealthComponent, ViewComponent, DieEvent>, Exc<DeadTag, InactiveTag>> _unitsFilter = default;
        readonly EcsFilterInject<Inc<HealthComponent, ViewComponent, EnemyTag>, Exc<DeadTag, InactiveTag>> _unitsEnemiesFilter = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<DeadTag> _deadPool = default;
        readonly EcsPoolInject<EnemyTag> _enemyPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<MainTowerTag> _mainTowerPool = default;
        readonly EcsPoolInject<Resurrectable> _resurrectablePool = default;
        readonly EcsPoolInject<RespawnEvent> _respawnEventPool = default;
        readonly EcsPoolInject<DropByDie> _dropPool = default;
        readonly EcsPoolInject<DropableItem> _dropableItemPool = default;
        readonly EcsPoolInject<ContextToolComponent> _contextToolPool = default;
        readonly EcsPoolInject<DestroyEffects> _destroyEffectsPool = default;

        readonly EcsPoolInject<ActivateContextToolEvent> _activateContextToolPool = default;
        readonly EcsPoolInject<DropEvent> _dropEventPool = default;
        readonly EcsPoolInject<LoseEvent> _losePool = default;
        readonly EcsPoolInject<DroppedGoldEvent> _goldPool = default;
        readonly EcsPoolInject<Player> _playerPool = default;
        readonly EcsPoolInject<CountdownWaveComponent> _countdownPool = default;

        readonly EcsFilterInject<Inc<TutorialComponent>> _filterTutor = default;

        readonly EcsPoolInject<CorpseRemove> _corpsePool = default;
        public void Run (EcsSystems systems)
        {
            foreach (var entity in _unitsFilter.Value)
            {
                if (_healthPool.Value.Get(entity).CurrentValue > 0)
                {
                    continue;
                }

                ref var viewComponent = ref _viewPool.Value.Get(entity);
                ref var interfaceComponent = ref _interfacePool.Value.Get(_state.Value.EntityInterface);
                if (viewComponent.GameObject)
                {
                    viewComponent.BaseLayer = viewComponent.GameObject.layer;
                    viewComponent.GameObject.layer = LayerMask.NameToLayer("Dead");
                }
                if (viewComponent.Rigidbody) viewComponent.Rigidbody.velocity = Vector3.zero;
                if (viewComponent.Animator)
                {
                    viewComponent.Animator.SetTrigger("Die");
                    viewComponent.Animator.SetLayerWeight(1, 0);
                }
                if (_playerPool.Value.Has(entity))
                {
                    //_activateContextToolPool
                }
                if (_enemyPool.Value.Has(entity))
                {
                    _state.Value.EnemiesWave--;
                    _state.Value.KillsCount++;
                    ref var goldComp = ref _goldPool.Value.Add(_world.Value.NewEntity());
                    if (viewComponent.Transform) goldComp.Position = viewComponent.Transform.position;
                    ref var corpseComp = ref _corpsePool.Value.Add(entity);
                    corpseComp.timer = 5f;
                    corpseComp.Entity = entity;
                    interfaceComponent._waveCounter.GetComponent<CounterMB>().sliders[_state.Value.GetCurrentWave()].value = (float)_state.Value.EnemiesWave / (float)_state.Value.StaticEnemiesWave; 
                    if (_state.Value.EnemiesWave == 0 && _state.Value.Saves.TutorialStage == 12 && _state.Value.GetCurrentWave() != _state.Value.WaveStorage.Waves.Count)
                    {
                        //interfaceComponent._waveCounter.GetComponent<CounterMB>().ChangeCount(_state.Value.GetCurrentWave());
                        _world.Value.GetPool<CountdownWaveComponent>().Add(_world.Value.NewEntity());
                        interfaceComponent.countdownWave.GetComponent<CountdownWaveMB>().SetTimer(_state.Value.TimeToNextWave);
                        interfaceComponent.countdownWave.GetComponent<CountdownWaveMB>().SwitcherTurn(true);
                        if (_state.Value.GetCurrentWave() == _state.Value.WaveStorage.Waves.Count - 2)
                            interfaceComponent.countdownWave.GetComponent<CountdownWaveMB>().SetText("Last wave!");
                    }
                }

                if (viewComponent.Outline) viewComponent.Outline.enabled = false;
                if (viewComponent.NavMeshAgent) viewComponent.NavMeshAgent.enabled = false;
                if (viewComponent.Healthbar) viewComponent.Healthbar.Disable();

                _deadPool.Value.Add(entity);

                //if (_enemyPool.Value.Has(entity)) interfaceComponent.progressbar.GetComponent<ProgressBarMB>().UpdateProgressBar();

                if (_mainTowerPool.Value.Has(entity)) _losePool.Value.Add(_world.Value.NewEntity());

                if (_resurrectablePool.Value.Has(entity))
                {
                    _respawnEventPool.Value.Add(entity);
                }

                if (_deadPool.Value.Has(_state.Value.EntityPlayer))
                {
                    _dropPool.Value.Add(_state.Value.EntityPlayer);
                }
                
                // Drop item
                if (_dropableItemPool.Value.Has(entity))
                {
                    ref var dropableItem = ref _dropableItemPool.Value.Get(entity);

                    ref var dropEvent = ref _dropEventPool.Value.Add(_world.Value.NewEntity());
                    dropEvent.Point = viewComponent.Transform.position;
                    dropEvent.Item = dropableItem.Item;
                }
                //start next wave 
                if (_state.Value.EnemiesWave == 0 && _state.Value.Saves.TutorialStage == 2)
                {
                    foreach (var tutor in _filterTutor.Value)
                    {
                        ref var tutorComp = ref _filterTutor.Pools.Inc1.Get(tutor);
                        tutorComp.TutorialStage = 3;
                        _state.Value.Saves.TutorialStage = 3;
                        _state.Value.Saves.SaveTutorial(3);
                    }
                }
                DeactivateTool(entity);

                ActivateDestroyExplosion(entity);
                _unitsFilter.Pools.Inc3.Del(entity);
            }
        }

        private void ActivateDestroyExplosion(in int entity)
        {
            if (!_destroyEffectsPool.Value.Has(entity)) return;

            ref var destroyEffectsComponent = ref _destroyEffectsPool.Value.Get(entity);
            destroyEffectsComponent.DestroyExplosion.Play();

            ref var viewComponent = ref _viewPool.Value.Get(entity);

            if (viewComponent.TowerWeapon == null) return;

            viewComponent.TowerWeapon.SetActive(false);
            viewComponent.TowerFirePoint.SetActive(false);
        }

        private void DeactivateTool(int entity)
        {
            if (!_contextToolPool.Value.Has(entity))
            {
                return;
            }

            ref var contextToolComponent = ref _contextToolPool.Value.Get(entity);
            ref var activateContextToolEvent = ref _activateContextToolPool.Value.Add(entity);

            activateContextToolEvent.ActiveTool = ContextToolComponent.Tool.empty;
        }
    }
}