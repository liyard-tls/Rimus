using System;

namespace Rimus.Scripts.Characters.Input
{
    public static class PlayerInputManager
    {
        public static BattlePlayerInput BattlePlayerInput;
        
        public static void Initialize()
        {
            BattlePlayerInput = new BattlePlayerInput();
        }
        
        public static void Dispose()
        {
            BattlePlayerInput.Dispose();
        }
    }
}