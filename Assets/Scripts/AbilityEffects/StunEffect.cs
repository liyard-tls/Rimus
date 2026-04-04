using System;

namespace AbilityEffects
{
    [Serializable]
    public class StunEffect: AbilityEffect
    {
        public float Duration;
        public ulong StartTime;
    }
}