using Rimus.Scripts.Characters;
using Rimus.Scripts.Characters.Managers;
using UnityEngine;

namespace Rimus.Scripts.Battle
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private CharacterEntity _playerCharacter;
        [SerializeField] private CharacterEntity _enemyCharacter;
        [SerializeField] private CharacterEntity _currentCharacter;

        private BattleTurnManager _battleTurnManager = new BattleTurnManager();
        private 
        
        void StartBattle()
        {
            _battleTurnManager
                .AddCharacter(_playerCharacter)
                .AddCharacter(_enemyCharacter);
            NextTurn();
        }

        void NextTurn()
        {
            _currentCharacter = _battleTurnManager.GetTurnCharacter();
        }
        
        void FinishTurn()
        {
            _battleTurnManager.CharacterFinishTurn(_currentCharacter);
            NextTurn();
        }

        void EndBattle()
        {
        }
    }
}