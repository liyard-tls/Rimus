using System.Collections.Generic;
using System.Text;
using Rimus.Scripts.Tools;
using UnityEngine;

namespace Rimus.Scripts.Characters.TargetSelection
{
    public class AttackSelector : MonoBehaviour
    {
        [SerializeField] private AttackSelectorType _currentSelectorType = AttackSelectorType.RadialSector;
        [SerializeField] private AttackSelectorView[] _selectors;
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField] private bool _keepOnlyTargetsInsideSelector = true;
        [SerializeField] private bool _debugTargetLogs = true;
        [SerializeField] private float _debugNearbySearchRadius = 1f;

        private AttackSelectorView _activeSelector;
        private readonly List<Targetable> _hoveredTargets = new List<Targetable>();
        private readonly List<Targetable> _selectedTargets = new List<Targetable>();
        private readonly HashSet<Targetable> _hoveredTargetSet = new HashSet<Targetable>();
        private readonly HashSet<Targetable> _selectedTargetSet = new HashSet<Targetable>();
        private string _lastDetectionLogKey;

        public IReadOnlyList<Targetable> HoveredTargets => _hoveredTargets;
        public IReadOnlyList<Targetable> SelectedTargets => _selectedTargets;
        public Targetable HoveredTarget => _hoveredTargets.Count > 0 ? _hoveredTargets[0] : null;
        public Targetable SelectedTarget => _selectedTargets.Count > 0 ? _selectedTargets[0] : null;
        public AttackSelectorView ActiveSelector => _activeSelector;

        private void Awake()
        {
            if (_selectors == null || _selectors.Length == 0)
            {
                _selectors = GetComponentsInChildren<AttackSelectorView>(true);
            }

            SetSelectorType(_currentSelectorType);
        }

        public void SetSelectorType(AttackSelectorType selectorType)
        {
            _currentSelectorType = selectorType;
            _activeSelector = null;

            if (_selectors == null)
            {
                return;
            }

            for (int i = 0; i < _selectors.Length; i++)
            {
                AttackSelectorView selector = _selectors[i];
                bool isActive = selector != null && selector.SelectorType == selectorType;
                if (isActive)
                {
                    _activeSelector = selector;
                }

                if (selector != null)
                {
                    selector.SetVisible(isActive);
                }
            }

            if (_debugTargetLogs)
            {
                Log.Info($"AttackSelector switched to selector type: {_currentSelectorType}. Active selector: {(_activeSelector != null ? _activeSelector.name : "null")}");
            }
        }

        public void UpdateTargeting(Vector3 targetPosition)
        {
            if (_activeSelector == null)
            {
                return;
            }

            _activeSelector.UpdateSelector(transform.position, targetPosition);
            List<Targetable> currentTargets = GetTargetsInSelector();
            SetHoveredTargets(currentTargets);

            if (_keepOnlyTargetsInsideSelector)
            {
                SyncSelectedTargetsWithHovered();
            }
        }

        public void ConfirmSelection(bool clearSelectionWhenNoTargets = true)
        {
            if (_hoveredTargets.Count > 0)
            {
                SetSelectedTargets(_hoveredTargets);
            }
            else if (clearSelectionWhenNoTargets)
            {
                SetSelectedTargets(null);
            }
        }

        public void ClearSelection()
        {
            SetSelectedTargets(null);
        }

        public void ClearHover()
        {
            SetHoveredTargets(null);
        }

        private void OnDisable()
        {
            ClearHover();
            ClearSelection();
        }

        private List<Targetable> GetTargetsInSelector()
        {
            if (_activeSelector == null)
            {
                return null;
            }

            Vector3 detectionCenter = _activeSelector.GetDetectionCenter();
            float detectionRadius = _activeSelector.GetDetectionRadius();
            Collider2D[] hits = Physics2D.OverlapCircleAll(detectionCenter, detectionRadius, _targetLayerMask);

            Dictionary<Targetable, float> candidates = new Dictionary<Targetable, float>();

            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D hit = hits[i];
                if (hit == null)
                {
                    continue;
                }

                Targetable candidate = hit.GetComponentInParent<Targetable>();
                if (candidate == null)
                {
                    LogDetectionOnce("target-missing-component-" + hit.GetInstanceID(), $"Collider '{hit.name}' is inside selector broad phase, but no Targetable component was found in parents.");
                    continue;
                }

                Vector3 closestPoint = hit.ClosestPoint(detectionCenter);
                if (!_activeSelector.ContainsWorldPoint(closestPoint))
                {
                    continue;
                }

                float candidateDistance = Vector2.SqrMagnitude((Vector2)(candidate.transform.position - transform.position));
                if (!candidates.TryGetValue(candidate, out float currentDistance) || candidateDistance < currentDistance)
                {
                    candidates[candidate] = candidateDistance;
                }
            }

            if (candidates.Count > 0)
            {
                List<KeyValuePair<Targetable, float>> orderedCandidates = new List<KeyValuePair<Targetable, float>>(candidates);
                orderedCandidates.Sort((left, right) => left.Value.CompareTo(right.Value));

                List<Targetable> targets = new List<Targetable>(orderedCandidates.Count);
                for (int i = 0; i < orderedCandidates.Count; i++)
                {
                    targets.Add(orderedCandidates[i].Key);
                }

                LogDetectionOnce("target-hit-" + targets.Count + "-" + targets[0].GetInstanceID(), $"Targets detected in selector. Count: {targets.Count}, Primary: {targets[0].name}, SelectorType: {_currentSelectorType}, DetectionCenter: {detectionCenter}, DetectionRadius: {detectionRadius}");
                return targets;
            }

            LogDetectionOnce("target-none", BuildNoTargetLog(detectionCenter, detectionRadius));
            return null;
        }

        private void SetHoveredTargets(IReadOnlyList<Targetable> targets)
        {
            if (AreSameTargets(_hoveredTargets, targets))
            {
                return;
            }

            if (_debugTargetLogs)
            {
                Log.Info($"Hovered targets changed from [{FormatTargets(_hoveredTargets)}] to [{FormatTargets(targets)}]");
            }

            for (int i = 0; i < _hoveredTargets.Count; i++)
            {
                Targetable oldTarget = _hoveredTargets[i];
                if (oldTarget != null && !_selectedTargetSet.Contains(oldTarget))
                {
                    oldTarget.SetOnTargeted(false);
                }
            }

            _hoveredTargets.Clear();
            _hoveredTargetSet.Clear();

            if (targets == null)
            {
                return;
            }

            for (int i = 0; i < targets.Count; i++)
            {
                Targetable target = targets[i];
                if (target == null || !_hoveredTargetSet.Add(target))
                {
                    continue;
                }

                _hoveredTargets.Add(target);
                target.SetOnTargeted(true);
            }
        }

        private void SetSelectedTargets(IReadOnlyList<Targetable> targets)
        {
            if (AreSameTargets(_selectedTargets, targets))
            {
                return;
            }

            if (_debugTargetLogs)
            {
                Log.Info($"Selected targets changed from [{FormatTargets(_selectedTargets)}] to [{FormatTargets(targets)}]");
            }

            for (int i = 0; i < _selectedTargets.Count; i++)
            {
                Targetable oldTarget = _selectedTargets[i];
                if (oldTarget != null && !_hoveredTargetSet.Contains(oldTarget))
                {
                    oldTarget.SetOnTargeted(false);
                }
            }

            _selectedTargets.Clear();
            _selectedTargetSet.Clear();

            if (targets == null)
            {
                return;
            }

            for (int i = 0; i < targets.Count; i++)
            {
                Targetable target = targets[i];
                if (target == null || !_selectedTargetSet.Add(target))
                {
                    continue;
                }

                _selectedTargets.Add(target);
                target.SetOnTargeted(true);
            }
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

        private void SyncSelectedTargetsWithHovered()
        {
            if (_selectedTargets.Count == 0)
            {
                return;
            }

            List<Targetable> remainingSelectedTargets = null;

            for (int i = 0; i < _selectedTargets.Count; i++)
            {
                Targetable selectedTarget = _selectedTargets[i];
                if (selectedTarget == null || !_hoveredTargetSet.Contains(selectedTarget))
                {
                    continue;
                }

                remainingSelectedTargets ??= new List<Targetable>();
                remainingSelectedTargets.Add(selectedTarget);
            }

            SetSelectedTargets(remainingSelectedTargets);
        }

        private string BuildNoTargetLog(Vector3 detectionCenter, float detectionRadius)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"No target detected in selector at {detectionCenter}. LayerMask: {_targetLayerMask.value}, SelectorRadius: {detectionRadius}.");

            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(detectionCenter, Mathf.Max(_debugNearbySearchRadius, detectionRadius));
            if (nearbyColliders == null || nearbyColliders.Length == 0)
            {
                builder.Append($" No Collider2D found within debug radius {Mathf.Max(_debugNearbySearchRadius, detectionRadius)}.");
                return builder.ToString();
            }

            builder.Append(" Nearby colliders: ");
            for (int i = 0; i < nearbyColliders.Length; i++)
            {
                Collider2D collider = nearbyColliders[i];
                if (collider == null)
                {
                    continue;
                }

                if (i > 0)
                {
                    builder.Append(" | ");
                }

                Targetable nearbyTargetable = collider.GetComponentInParent<Targetable>();
                Vector3 closestPoint = collider.ClosestPoint(detectionCenter);
                bool insideSelector = _activeSelector != null && _activeSelector.ContainsWorldPoint(closestPoint);
                float distance = Vector2.Distance(detectionCenter, closestPoint);
                builder.Append($"'{collider.name}' layer={LayerMask.LayerToName(collider.gameObject.layer)}({collider.gameObject.layer}) targetable={(nearbyTargetable != null ? nearbyTargetable.name : "null")} dist={distance:0.###} insideSelector={insideSelector}");
            }

            return builder.ToString();
        }

        private static bool AreSameTargets(IReadOnlyList<Targetable> currentTargets, IReadOnlyList<Targetable> newTargets)
        {
            int newCount = newTargets != null ? newTargets.Count : 0;
            if (currentTargets.Count != newCount)
            {
                return false;
            }

            for (int i = 0; i < currentTargets.Count; i++)
            {
                if (currentTargets[i] != newTargets[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static string FormatTargets(IReadOnlyList<Targetable> targets)
        {
            if (targets == null || targets.Count == 0)
            {
                return "none";
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < targets.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(targets[i] != null ? targets[i].name : "null");
            }

            return builder.ToString();
        }
    }
}
