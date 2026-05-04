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
        
        public void StartBattle()
        {
            _battleTurnManager
                .AddCharacter(_playerCharacter)
                .AddCharacter(_enemyCharacter);
            NextTurn();
        }

        public void NextTurn()
        {
            _currentCharacter = _battleTurnManager.GetTurnCharacter();
            _currentCharacter.StartTurn();
        }
        
        public void FinishTurn()
        {
            _battleTurnManager.CharacterFinishTurn(_currentCharacter);
            NextTurn();
        }

        public void EndBattle()
        {
        }
    }
}