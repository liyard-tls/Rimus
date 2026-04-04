using System;

namespace AbilityEffects
{
    [Serializable]
    public class PeriodicalDamage: AbilityEffect
    {
        public float Value;
        public float Duration;
        public float TickRate;
    }
}