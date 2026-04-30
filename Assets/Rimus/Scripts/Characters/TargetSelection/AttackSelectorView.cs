using UnityEngine;

namespace Rimus.Scripts.Characters.TargetSelection
{
    public abstract class AttackSelectorView : MonoBehaviour
    {
        public abstract AttackSelectorType SelectorType { get; }

        public virtual void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public abstract void UpdateSelector(Vector3 sourcePosition, Vector3 targetPosition);
        public abstract Vector3 GetDetectionCenter();
        public abstract float GetDetectionRadius();
        public abstract bool ContainsWorldPoint(Vector3 worldPoint);
    }
}
