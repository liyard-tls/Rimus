using System.Collections.Generic;
using AbilityEffects;
using CMS;
using Sirenix.Serialization;

namespace Abilities
{
    public class Ability: Entity
    {
        [OdinSerialize]
        public List<AbilityEffect> Effects = new List<AbilityEffect>();
    }
}