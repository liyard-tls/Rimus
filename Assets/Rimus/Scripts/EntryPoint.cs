using System;
using Configs;
using Rimus.Scripts.Battle;
using Rimus.Scripts.Characters.Input;
using UnityEngine;

namespace Rimus.Scripts
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private BattleManager _battleManager;
        public void Start()
        {
            Initialize();
            StartBattle();
        }

        private void Initialize()
        {
            GlobalContext.BattleManager = _battleManager;
            PlayerInputManager.Initialize();
        }

        private void StartBattle()
        {
            GlobalContext.BattleManager.StartBattle();
        }

        public void OnDestroy()
        {
            PlayerInputManager.Dispose();
        }
    }
}