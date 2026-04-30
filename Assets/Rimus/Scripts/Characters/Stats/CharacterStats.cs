using System;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    [Serializable]
    public struct CharacterStats
    {
        public static CharacterStats Default => new CharacterStats
        {
            MaxHp = 10,
            MaxMp = 0,
            Attack = 1,
            MagicAttack = 1,
            Defense = 0,
            MagicDefense = 0,
            Speed = 1,
            Accuracy = 100,
            Evasion = 0,
            CritChance = 0,
            CritDamage = 150
        };

        [Min(1)] public int MaxHp;
        [Min(0)] public int MaxMp;
        [Min(0)] public int Attack;
        [Min(0)] public int MagicAttack;
        [Min(0)] public int Defense;
        [Min(0)] public int MagicDefense;
        [Min(0)] public int Speed;
        [Range(0, 100)] public int Accuracy;
        [Range(0, 100)] public int Evasion;
        [Range(0, 100)] public int CritChance;
        [Min(100)] public int CritDamage;

        public static CharacterStats operator +(CharacterStats left, CharacterStats right)
        {
            return new CharacterStats
            {
                MaxHp = left.MaxHp + right.MaxHp,
                MaxMp = left.MaxMp + right.MaxMp,
                Attack = left.Attack + right.Attack,
                MagicAttack = left.MagicAttack + right.MagicAttack,
                Defense = left.Defense + right.Defense,
                MagicDefense = left.MagicDefense + right.MagicDefense,
                Speed = left.Speed + right.Speed,
                Accuracy = left.Accuracy + right.Accuracy,
                Evasion = left.Evasion + right.Evasion,
                CritChance = left.CritChance + right.CritChance,
                CritDamage = left.CritDamage + right.CritDamage
            };
        }

        public static CharacterStats Scale(CharacterStats value, float multiplier)
        {
            return new CharacterStats
            {
                MaxHp = Mathf.RoundToInt(value.MaxHp * multiplier),
                MaxMp = Mathf.RoundToInt(value.MaxMp * multiplier),
                Attack = Mathf.RoundToInt(value.Attack * multiplier),
                MagicAttack = Mathf.RoundToInt(value.MagicAttack * multiplier),
                Defense = Mathf.RoundToInt(value.Defense * multiplier),
                MagicDefense = Mathf.RoundToInt(value.MagicDefense * multiplier),
                Speed = Mathf.RoundToInt(value.Speed * multiplier),
                Accuracy = Mathf.RoundToInt(value.Accuracy * multiplier),
                Evasion = Mathf.RoundToInt(value.Evasion * multiplier),
                CritChance = Mathf.RoundToInt(value.CritChance * multiplier),
                CritDamage = Mathf.RoundToInt(value.CritDamage * multiplier)
            };
        }

        public CharacterStats ClampMinimums()
        {
            MaxHp = Mathf.Max(1, MaxHp);
            MaxMp = Mathf.Max(0, MaxMp);
            Attack = Mathf.Max(0, Attack);
            MagicAttack = Mathf.Max(0, MagicAttack);
            Defense = Mathf.Max(0, Defense);
            MagicDefense = Mathf.Max(0, MagicDefense);
            Speed = Mathf.Max(0, Speed);
            Accuracy = Mathf.Clamp(Accuracy, 0, 100);
            Evasion = Mathf.Clamp(Evasion, 0, 100);
            CritChance = Mathf.Clamp(CritChance, 0, 100);
            CritDamage = Mathf.Max(100, CritDamage);
            return this;
        }
    }
}
