using UnityEngine;

namespace Rimus.Scripts.Inbox
{
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererCircleSelector : AttackSelectorView
    {
        [SerializeField] private float radius = 1.5f;
        [SerializeField] private int segments = 48;
        [SerializeField] private bool useLocalSpace = true;
        [SerializeField] private float depthOffset = 0.02f;
        [SerializeField] private Vector2 visualScale = Vector2.one;

        private LineRenderer _lineRenderer;

        public override AttackSelectorType SelectorType => AttackSelectorType.Circle;

        public override Vector3 GetDetectionCenter()
        {
            return transform.position;
        }

        public override float GetDetectionRadius()
        {
            return radius * Mathf.Max(visualScale.x, visualScale.y);
        }

        private void Awake()
        {
            CacheLineRenderer();
            Refresh();
        }

        private void OnEnable()
        {
            CacheLineRenderer();
            Refresh();
        }

        private void OnValidate()
        {
            radius = Mathf.Max(0f, radius);
            segments = Mathf.Max(3, segments);
            visualScale.x = Mathf.Max(0f, visualScale.x);
            visualScale.y = Mathf.Max(0f, visualScale.y);

            CacheLineRenderer();
            Refresh();
        }

        public override void UpdateSelector(Vector3 sourcePosition, Vector3 targetPosition)
        {
            transform.position = targetPosition;
            Refresh();
        }

        public override bool ContainsWorldPoint(Vector3 worldPoint)
        {
            Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
            float scaleX = visualScale.x > Mathf.Epsilon ? visualScale.x : 1f;
            float scaleY = visualScale.y > Mathf.Epsilon ? visualScale.y : 1f;
            float normalizedX = localPoint.x / (radius * scaleX);
            float normalizedY = localPoint.y / (radius * scaleY);
            return (normalizedX * normalizedX) + (normalizedY * normalizedY) <= 1f;
        }

        private void CacheLineRenderer()
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }

            _lineRenderer.loop = true;
            _lineRenderer.useWorldSpace = !useLocalSpace;
        }

        private void Refresh()
        {
            if (_lineRenderer == null)
            {
                return;
            }

            _lineRenderer.useWorldSpace = !useLocalSpace;

            Vector3[] points = BuildPoints();
            _lineRenderer.positionCount = points.Length;
            _lineRenderer.SetPositions(points);
        }

        private Vector3[] BuildPoints()
        {
            Vector3[] points = new Vector3[segments];

            for (int i = 0; i < segments; i++)
            {
                float angle = i / (float)segments * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * radius * visualScale.x;
                float y = Mathf.Sin(angle) * radius * visualScale.y;
                points[i] = ApplySpace(new Vector3(x, y, depthOffset));
            }

            return points;
        }

        private Vector3 ApplySpace(Vector3 point)
        {
            return useLocalSpace ? point : transform.TransformPoint(point);
        }
    }
}
