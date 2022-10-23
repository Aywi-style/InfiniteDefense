namespace Client
{
    struct VibrationEvent
    {
        public enum VibrationType
        {
            HeavyImpact,
            LightImpact,
            MediumImpact,
            RigidImpact,
            Selection,
            SoftImpact,
            Success,
            Warning
        }

        public VibrationType Vibration;
    }
}