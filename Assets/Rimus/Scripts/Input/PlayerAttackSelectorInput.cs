using Rimus.Scripts.Characters;
using Rimus.Scripts.Characters.TargetSelection;
using Rimus.Scripts.Tools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rimus.Scripts.Input
{
    public class PlayerAttackSelectorInput : MonoBehaviour
    {
        [SerializeField] private AttackSelector _attackSelector;
        [SerializeField] private SkillCaster _skillCaster;
        [SerializeField] private Camera _camera;
        [SerializeField] private float _planeDepth;
        [SerializeField] private bool _useTransformDepth = true;
        [SerializeField] private bool _clearSelectionWhenClickingEmptySpace = true;
        [SerializeField] private bool _castOnLeftClick;
        [SerializeField] private bool _debugTargetLogs = true;
        [SerializeField] private float _debugNearbySearchRadius = 1f;
        [SerializeField] private bool _debugVerboseMouseLogs = true;
        [SerializeField] private bool _clearHoverOnDisable = true;

        private string _lastDetectionLogKey;
        private bool _isActive = false;

        private void Awake()
        {
            if (_attackSelector == null)
            {
                _attackSelector = GetComponent<AttackSelector>();
            }

            if (_skillCaster == null)
            {
                _skillCaster = GetComponent<SkillCaster>();
            }

            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        public void SetActive(bool active)
        {
            _isActive = active;
            if (!active && _attackSelector != null && _clearHoverOnDisable)
            {
                _attackSelector.ClearHover();
            }
        }

        private void Update()
        {
            if (!_isActive && _attackSelector == null || _camera == null)
            {
                return;
            }

            Vector3 mouseWorldPosition;
            if (!TryGetMouseWorldPosition(out mouseWorldPosition))
            {
                return;
            }

            _attackSelector.UpdateTargeting(mouseWorldPosition);

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                _attackSelector.ConfirmSelection(_clearSelectionWhenClickingEmptySpace);

                if (_castOnLeftClick && _skillCaster != null)
                {
                    _skillCaster.TryCastCurrentSkill();
                }
            }
        }

        private void OnDisable()
        {
            if (_attackSelector == null)
            {
                return;
            }

            if (_clearHoverOnDisable)
            {
                _attackSelector.ClearHover();
            }
        }

        private bool TryGetMouseWorldPosition(out Vector3 worldPosition)
        {
            if (Mouse.current == null)
            {
                LogDetectionOnce("mouse-null", "Mouse.current is null. Target detection skipped.");
                worldPosition = default;
                return false;
            }

            float planeDepth = _useTransformDepth ? _attackSelector.transform.position.z : _planeDepth;
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(mouseScreenPosition);
            Plane groundPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, planeDepth));

            float enter;
            if (groundPlane.Raycast(ray, out enter))
            {
                worldPosition = ray.GetPoint(enter);
                LogMouseWorldPosition(mouseScreenPosition, planeDepth, ray, enter, worldPosition);
                return true;
            }

            worldPosition = default;
            LogDetectionOnce("mouse-plane-miss", $"Mouse ray did not hit target plane at depth {planeDepth}.");
            return false;
        }

        private void LogDetectionOnce(string key, string message)
        {
            if (!_debugTargetLogs || _lastDetectionLogKey == key)
            {
                return;
            }

            _lastDetectionLogKey = key;
            Log.Info(message);
        }

        private void LogMouseWorldPosition(Vector2 mouseScreenPosition, float planeDepth, Ray ray, float enter, Vector3 worldPosition)
        {
            if (!_debugTargetLogs || !_debugVerboseMouseLogs)
            {
                return;
            }

            Log.Info(
                $"Mouse screen={mouseScreenPosition}, planeDepth={planeDepth}, cameraPos={_camera.transform.position}, " +
                $"cameraRot={_camera.transform.rotation.eulerAngles}, rayOrigin={ray.origin}, rayDir={ray.direction}, " +
                $"enter={enter:0.###}, world={worldPosition}, overlapCount={Physics2D.OverlapCircleAll(worldPosition, _debugNearbySearchRadius).Length}.");
        }
    }
}
