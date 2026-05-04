using Configs;
using UnityEngine.InputSystem;

namespace Rimus.Scripts.Characters.Input
{
    public class BattlePlayerInput
    {
        private InputSystem_Actions _inputSystem;

        public BattlePlayerInput()
        {
            _inputSystem = new InputSystem_Actions();
            _inputSystem.Battle.UseSkill1.performed += UseSkill1;
            _inputSystem.Battle.UseSkill2.performed += UseSkill2;
            _inputSystem.Battle.UseSkill3.performed += UseSkill3;
            _inputSystem.Battle.UseSkill4.performed += UseSkill4;
            _inputSystem.Battle.SimpleAttack.performed += SimpleAttack;
            _inputSystem.Battle.Discard.performed += Discard;
            _inputSystem.Battle.UseItem.performed += UseItem;
        }
        
        public void SetActive(bool active)
        {
            if (active)
                _inputSystem.Battle.Enable();
            else
                _inputSystem.Battle.Disable();
        }

        private void UseSkill1(InputAction.CallbackContext context) => UseSkill(0);
        private void UseSkill2(InputAction.CallbackContext context) => UseSkill(1);
        private void UseSkill3(InputAction.CallbackContext context) => UseSkill(2);
        private void UseSkill4(InputAction.CallbackContext context) => UseSkill(3);
        
        private void SimpleAttack(InputAction.CallbackContext context)
        {
        }
        
        private void Discard(InputAction.CallbackContext context)
        {
        }

        private void UseItem(InputAction.CallbackContext context)
        {
        }
        
        public void UseSkill(int index)
        {
        }
        
        public void Dispose()
        {
            _inputSystem.Battle.UseSkill1.performed -= UseSkill1;
            _inputSystem.Battle.UseSkill2.performed -= UseSkill2;
            _inputSystem.Battle.UseSkill3.performed -= UseSkill3;
            _inputSystem.Battle.UseSkill4.performed -= UseSkill4;
            _inputSystem.Battle.SimpleAttack.performed -= SimpleAttack;
            _inputSystem.Battle.Discard.performed -= Discard;
            _inputSystem.Battle.UseItem.performed -= UseItem;
        }
    }
}