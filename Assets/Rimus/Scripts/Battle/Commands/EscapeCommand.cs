using System.Collections;
using Rimus.Scripts.Battle.Interfaces;
using Rimus.Scripts.Characters;

namespace Rimus.Scripts.Battle.Commands
{
    public class EscapeCommand: IBattleCommand
    {
        private readonly CharacterEntity _attacker;
        private readonly CharacterEntity _target;
        
        public EscapeCommand(CharacterEntity attacker, CharacterEntity target)
        {
            _attacker = attacker;
            _target = target;
        }
        
        public IEnumerator Execute(BattleContext context)
        {
            var damage = _attacker.Stats.CurrentStats.Attack;
            _target.Health.TakeDamage(damage);
            yield return null;
        }
    }
}