using AbilityEffects;

namespace Abilities
{
    public class PoisonStunAbility: Ability
    {
        public PoisonStunAbility()
        {
            var poisonEffect = new PeriodicalDamage()
            {
                Duration = 5,
                TickRate = 1,
                Value = 10
            };
            
            var stunEffect = new StunEffect
            {
                Duration = 2,
                StartTime = 0
            };
            
            Effects.Add(poisonEffect);
            Effects.Add(stunEffect);
        }
    }
}