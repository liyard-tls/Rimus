namespace Rimus.Scripts.Characters
{
    public class CharacterTurnHandler
    {
        private bool _isControlledByPlayer;
        public CharacterTurnHandler(bool isControlledByPlayer)
        {
            _isControlledByPlayer = isControlledByPlayer;
        }
    }
}