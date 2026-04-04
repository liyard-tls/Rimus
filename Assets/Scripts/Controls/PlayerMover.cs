using Configs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controls
{
    [RequireComponent(typeof(CharacterControllerMover))]
    public class PlayerMover : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        private InputSystem_Actions _input;
        private CharacterControllerMover _mover;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool SprintHeld  { get; private set; }
        public bool CrouchHeld  { get; private set; }

        private void Awake()
        {
            _mover = GetComponent<CharacterControllerMover>();
            _input = new InputSystem_Actions();
            _input.Player.AddCallbacks(this);
        }

        private void Update()
        {
            var direction = new Vector3(MoveInput.x, 0f, MoveInput.y);
            _mover.Move(direction);
        }

        private void OnEnable()  => _input.Player.Enable();
        private void OnDisable() => _input.Player.Disable();
        private void OnDestroy() => _input.Dispose();

        // --- IPlayerActions ---

        public void OnMove(InputAction.CallbackContext ctx)
            => MoveInput = ctx.ReadValue<Vector2>();

        public void OnLook(InputAction.CallbackContext ctx)
            => LookInput = ctx.ReadValue<Vector2>();

        public void OnJump(InputAction.CallbackContext ctx)
            => JumpPressed = ctx.performed;

        public void OnSprint(InputAction.CallbackContext ctx)
            => SprintHeld = ctx.performed;

        public void OnCrouch(InputAction.CallbackContext ctx)
            => CrouchHeld = ctx.performed;

        public void OnAttack(InputAction.CallbackContext ctx)   { }
        public void OnInteract(InputAction.CallbackContext ctx) { }
        public void OnPrevious(InputAction.CallbackContext ctx) { }
        public void OnNext(InputAction.CallbackContext ctx)     { }
    }
}