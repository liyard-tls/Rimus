using System;
using System.Collections.Generic;
using Rimus.Scripts.Tools.GameContentSystem;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    [CreateAssetMenu(menuName = "Rimus/Characters/Character Definition")]
    public class CharacterDefinition : ScriptableObject, IGameContent
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField, Min(1)] private int _startingLevel = 1;
        [SerializeField] private CharacterStats _baseStats = CharacterStats.Default;
        [SerializeField] private List<SkillDefinition> _defaultSkills = new List<SkillDefinition>();

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string DisplayName => string.IsNullOrWhiteSpace(_displayName) ? name : _displayName;
        public int StartingLevel => _startingLevel;
        public CharacterStats BaseStats => _baseStats;
        public IReadOnlyList<SkillDefinition> DefaultSkills => _defaultSkills;
    }
}
