using System.Collections;
using Rimus.Scripts.Battle.Interfaces;
using Rimus.Scripts.Characters;
using UnityEngine;

namespace Rimus.Scripts.Battle.Commands
{
    public class AttackCommand: IBattleCommand
    {
        private readonly CharacterEntity _attacker;
        private readonly CharacterEntity _target;
        
        public AttackCommand(CharacterEntity attacker, CharacterEntity target)
        {
            _attacker = attacker;
            _target = target;
        }
        
        public IEnumerator Execute(BattleContext context)
        {
            if (_attacker.Health.IsDead)
            {
                yield break;
            }
            var damage = context.BattleCalculator.CalculatePhysicalDamage(_attacker, _target);
            context.BattleLog.AddEntry(this, $"{_attacker.Definition.DisplayName} attacks {_target.Definition.DisplayName} for {damage} damage.");
            
            SkillDefinition skillDef = _attacker.SkillCaster.CurrentSkill;
            _attacker.View.PlayAnimation(skillDef.AnimationName);
            yield return new WaitForSeconds(skillDef.AnimationActionTime);
            _target.Health.TakeDamage(damage);
            yield return new WaitForSeconds(skillDef.AnimationDuration - skillDef.AnimationActionTime);
        }
    }
}