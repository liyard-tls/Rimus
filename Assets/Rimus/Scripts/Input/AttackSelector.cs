using Configs;
using Rimus.Scripts.Inbox;
using Rimus.Scripts.Tools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rimus.Scripts.Input
{
    public class AttackSelector : MonoBehaviour
    {
        [SerializeField] private LineRendererRadialSelector _radialSelector;
        [SerializeField] private Camera _camera;
        [SerializeField] private InputSystem_Actions _inputSystemActions;
        [SerializeField] private float _planeDepth;
        [SerializeField] private bool _useTransformDepth = true;

        private void Awake()
        {
            if (_radialSelector == null)
            {
                _radialSelector = GetComponentInChildren<LineRendererRadialSelector>();
            }

            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        private void Update()
        {
            if (_radialSelector == null || _camera == null)
            {
                return;
            }

            Vector3 mouseWorldPosition;
            if (!TryGetMouseWorldPosition(out mouseWorldPosition))
            {
                return;
            }
            
            Vector3 direction = mouseWorldPosition - transform.position;
            direction.z = 0f;

            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            _radialSelector.SetDirection(direction);
        }

        private bool TryGetMouseWorldPosition(out Vector3 worldPosition)
        {
            if (Mouse.current == null)
            {
                worldPosition = default;
                return false;
            }

            float planeDepth = _useTransformDepth ? transform.position.z : _planeDepth;
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

            if (_camera.orthographic)
            {
                float distanceFromCamera = planeDepth - _camera.transform.position.z;
                Vector3 screenPoint = new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, distanceFromCamera);
                worldPosition = _camera.ScreenToWorldPoint(screenPoint);
                worldPosition.z = planeDepth;
                return true;
            }

            Ray ray = _camera.ScreenPointToRay(mouseScreenPosition);
            Plane groundPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, planeDepth));

            float enter;
            if (groundPlane.Raycast(ray, out enter))
            {
                worldPosition = ray.GetPoint(enter);
                return true;
            }

            worldPosition = default;
            return false;
        }
    }
}
