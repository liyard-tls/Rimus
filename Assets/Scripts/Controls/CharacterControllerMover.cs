using System.Collections;
using UnityEngine;

namespace Controls
{
    // General class for all characters that use CharacterController to move. 
    // Not only for player, but also for NPCs.
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerMover : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _speed = 5f;
        
        [SerializeField] private float _epsilon = 0.1f; // Minimum distance to consider as "reached" the target
        
        private Coroutine _moveCoroutine;
        private float _verticalVelocity;

        public void MoveToPosition(Vector3 targetPosition)
        {
            StopMoving();
            _moveCoroutine = StartCoroutine(MoveToPositionCoroutine(targetPosition));
        }
        
        public void FollowTarget(Transform target)
        {
            StopMoving();
            _moveCoroutine = StartCoroutine(FollowTargetCoroutine(target));
        }
        
        public void StopMoving()
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }
        }
        
        private IEnumerator FollowTargetCoroutine(Transform target)
        {
            while (target != null)
            {
                Vector3 direction = target.position - transform.position;
                Move(direction);
                yield return null; // Wait for the next frame
            }
        }
        
        private IEnumerator MoveToPositionCoroutine(Vector3 targetPosition)
        {
            while (Vector3.Distance(transform.position, targetPosition) > _epsilon)
            {
                Vector3 direction = targetPosition - transform.position;
                Move(direction);
                yield return null; // Wait for the next frame
            }
        }
        
        public void Move(Vector3 direction)
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;
            else
                _verticalVelocity += Physics.gravity.y * Time.deltaTime;

            Vector3 movement = direction.normalized * _speed * Time.deltaTime;
            movement.y = _verticalVelocity * Time.deltaTime;
            _characterController.Move(movement);
        }
        
        
    }
}