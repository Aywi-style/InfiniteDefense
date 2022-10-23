using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Lofelt.NiceVibrations;
using UnityEngine;

namespace Client
{
    sealed class VibrationEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<VibrationEvent>> _vibrationEventFilter = default;

        readonly EcsPoolInject<VibrationEvent> _vibrationEventPool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var eventEntity in _vibrationEventFilter.Value)
            {
                ref var vibrationComponent = ref _vibrationEventPool.Value.Get(eventEntity);

                switch (vibrationComponent.Vibration)
                {
                    case VibrationEvent.VibrationType.HeavyImpact:
                        Lofelt.NiceVibrations.HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
                        break;
                    case VibrationEvent.VibrationType.LightImpact:
                        Lofelt.NiceVibrations.HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
                        break;
                    case VibrationEvent.VibrationType.MediumImpact:
                        Lofelt.NiceVibrations.HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
                        break;
                    case VibrationEvent.VibrationType.RigidImpact:
                        Lofelt.NiceVibrations.HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
                        break;
                    case VibrationEvent.VibrationType.Selection:
                        Lofelt.NiceVibrations.HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
                        break;
                    case VibrationEvent.VibrationType.SoftImpact:
                        Lofelt.NiceVibrations.HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
                        break;
                    case VibrationEvent.VibrationType.Success:
                        Lofelt.NiceVibrations.HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
                        break;
                    case VibrationEvent.VibrationType.Warning:
                        Lofelt.NiceVibrations.HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
                        break;
                }

                Debug.Log("Произошла вибрация "+vibrationComponent.Vibration);

                _vibrationEventPool.Value.Del(eventEntity);
            }
        }
    }
}