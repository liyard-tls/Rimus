using Rimus.Scripts.Characters;

namespace Rimus.Scripts.Battle
{
    public class BattleCalculator
    {
        public int CalculatePhysicalDamage(CharacterEntity attacker, CharacterEntity defender)
        {
            int attackerDamage = attacker.Stats.CurrentStats.Attack;
            int defenderDefense = defender.Stats.CurrentStats.Defense;
            int damage = attackerDamage - defenderDefense;
            return damage > 0 ? damage : 0;
        }
    }
}