using System.Collections.Generic;
using UnityEngine;

namespace Rimus.Scripts.Characters.Managers
{
    public class BattleTurnManager
    {
        private Dictionary<CharacterEntity, float> _characters;
        
        public BattleTurnManager AddCharacter(CharacterEntity character)
        {
            if (!_characters.ContainsKey(character))
            {
                _characters.Add(character, Time.time);
            }
            return this;
        }

        public CharacterEntity GetTurnCharacter()
        {
            // find the character with the longest time since last turn and highest speed
            CharacterEntity turnCharacter = null;
            float longestTime = -1f;
            foreach (var kvp in _characters)
            {
                var character = kvp.Key;
                var lastTurnTime = kvp.Value;
                var timeSinceLastTurn = Time.time - lastTurnTime;
                if (timeSinceLastTurn > longestTime || (Mathf.Approximately(timeSinceLastTurn, longestTime) &&
                                                        character.Stats.CurrentStats.Speed >
                                                        turnCharacter.Stats.CurrentStats.Speed))
                {
                    turnCharacter = character;
                    longestTime = timeSinceLastTurn;
                }
            }
            return turnCharacter;
        }

        public void CharacterFinishTurn(CharacterEntity character)
        {
            if (_characters.ContainsKey(character))
            {
                _characters[character] = Time.time;
            }
        }
    }
}