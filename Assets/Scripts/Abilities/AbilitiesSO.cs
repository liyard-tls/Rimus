using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "Abilities", menuName = "Content/Abilities")]
    public class AbilitiesSO : SerializedScriptableObject
    {
        [OdinSerialize]
        public Ability[] Abilities;
    }
}