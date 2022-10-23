using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ActivateContextToolEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<ContextToolComponent, ActivateContextToolEvent, ViewComponent>> _ActivateContextToolFilter = default;
        readonly EcsPoolInject<ContextToolComponent> _contextToolPool = default;
        readonly EcsPoolInject<ActivateContextToolEvent> _activateContextToolPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var playerEntity in _ActivateContextToolFilter.Value)
            {
                ref var contextToolComponent = ref _contextToolPool.Value.Get(playerEntity);
                ref var activateContextToolComponent = ref _activateContextToolPool.Value.Get(playerEntity);
                ref var viewComponent = ref _viewPool.Value.Get(playerEntity);

                if (activateContextToolComponent.ActiveTool == ContextToolComponent.Tool.pickaxe)
                {
                    if (contextToolComponent.CurrentActiveTool == ContextToolComponent.Tool.sword || contextToolComponent.CurrentActiveTool == ContextToolComponent.Tool.bow)
                    {
                        _activateContextToolPool.Value.Del(playerEntity);
                        continue;
                    }
                }

                if (contextToolComponent.CurrentActiveTool == ContextToolComponent.Tool.sword && activateContextToolComponent.ActiveTool == ContextToolComponent.Tool.empty)
                {
                    if (viewComponent.Animator.GetBool("Range"))
                    {
                        activateContextToolComponent.ActiveTool = ContextToolComponent.Tool.bow;
                    }
                }


                if (contextToolComponent.CurrentActiveTool != ContextToolComponent.Tool.empty)
                {
                    contextToolComponent.ToolsPool[((int)contextToolComponent.CurrentActiveTool)].SetActive(false);
                }

                if (activateContextToolComponent.ActiveTool == ContextToolComponent.Tool.empty)
                {
                    viewComponent.Animator.SetLayerWeight(1, 0);
                }
                else
                {
                    contextToolComponent.ToolsPool[((int)activateContextToolComponent.ActiveTool)].SetActive(true);

                    viewComponent.Animator.SetLayerWeight(1, 1);
                }

                contextToolComponent.CurrentActiveTool = activateContextToolComponent.ActiveTool;

                _activateContextToolPool.Value.Del(playerEntity);
            }
        }
    }
}