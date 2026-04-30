using UnityEngine;

namespace Rimus.Scripts.Inbox
{
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererRadialSelector : AttackSelectorView
    {
        [Header("Shape")]
        [SerializeField] private float radius = 4f;
        [SerializeField, Range(0f, 360f)] private float angle = 90f;
        [SerializeField] private float directionDegrees;
        [SerializeField] private float innerRadius;
        [SerializeField] private int arcSegments = 24;

        [Header("Projection")]
        [SerializeField] private bool useLocalSpace = true;
        [SerializeField] private Vector3 planeRight = Vector3.right;
        [SerializeField] private Vector3 planeForward = new Vector3(0f, 1f, 0f);
        [SerializeField] private float heightOffset = 0.02f;
        [SerializeField] private Vector2 visualScale = Vector2.one;

        private LineRenderer _lineRenderer;

        public override AttackSelectorType SelectorType => AttackSelectorType.RadialSector;

        public override Vector3 GetDetectionCenter()
        {
            return transform.position;
        }

        public override float GetDetectionRadius()
        {
            return radius * Mathf.Max(visualScale.x, visualScale.y);
        }

        public float Radius
        {
            get => radius;
            set
            {
                radius = Mathf.Max(0f, value);
                Refresh();
            }
        }

        public float Angle
        {
            get => angle;
            set
            {
                angle = Mathf.Clamp(value, 0f, 360f);
                Refresh();
            }
        }

        public float DirectionDegrees
        {
            get => directionDegrees;
            set
            {
                directionDegrees = value;
                Refresh();
            }
        }

        public float InnerRadius
        {
            get => innerRadius;
            set
            {
                innerRadius = Mathf.Max(0f, value);
                Refresh();
            }
        }

        public void SetDirection(Vector3 worldDirection)
        {
            Vector3 direction = useLocalSpace ? transform.InverseTransformDirection(worldDirection) : worldDirection;
            Vector2 planarDirection = new Vector2(ProjectToPlaneAxis(direction, PlaneRight), ProjectToPlaneAxis(direction, PlaneForward));
            planarDirection = CompensateForVisualScale(planarDirection);
            if (planarDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            directionDegrees = Mathf.Atan2(planarDirection.y, planarDirection.x) * Mathf.Rad2Deg;
            Refresh();
        }

        public void SetShape(float newRadius, float newAngle, float newDirectionDegrees)
        {
            radius = Mathf.Max(0f, newRadius);
            angle = Mathf.Clamp(newAngle, 0f, 360f);
            directionDegrees = newDirectionDegrees;
            Refresh();
        }

        private Vector3 PlaneRight => planeRight.sqrMagnitude > Mathf.Epsilon ? planeRight.normalized : Vector3.right;

        private Vector3 PlaneForward
        {
            get
            {
                Vector3 normalized = planeForward.sqrMagnitude > Mathf.Epsilon ? planeForward.normalized : Vector3.up;
                Vector3 orthogonalized = normalized - Vector3.Dot(normalized, PlaneRight) * PlaneRight;
                return orthogonalized.sqrMagnitude > Mathf.Epsilon ? orthogonalized.normalized : Vector3.up;
            }
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
            angle = Mathf.Clamp(angle, 0f, 360f);
            arcSegments = Mathf.Max(3, arcSegments);
            innerRadius = Mathf.Clamp(innerRadius, 0f, radius);
            visualScale.x = Mathf.Max(0f, visualScale.x);
            visualScale.y = Mathf.Max(0f, visualScale.y);

            CacheLineRenderer();
            Refresh();
        }

        private void CacheLineRenderer()
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }

            _lineRenderer.loop = false;
            _lineRenderer.useWorldSpace = !useLocalSpace;
        }

        public override void UpdateSelector(Vector3 sourcePosition, Vector3 targetPosition)
        {
            transform.position = sourcePosition;

            Vector3 direction = targetPosition - sourcePosition;
            direction.z = 0f;

            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            SetDirection(direction);
        }

        public override bool ContainsWorldPoint(Vector3 worldPoint)
        {
            Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
            Vector2 planarPoint = new Vector2(ProjectToPlaneAxis(localPoint, PlaneRight), ProjectToPlaneAxis(localPoint, PlaneForward));
            planarPoint = CompensateForVisualScale(planarPoint);

            float distance = planarPoint.magnitude;
            if (distance < innerRadius || distance > radius)
            {
                return false;
            }

            if (angle >= 360f)
            {
                return true;
            }

            float pointAngle = Mathf.Atan2(planarPoint.y, planarPoint.x) * Mathf.Rad2Deg;
            float relativeAngle = Mathf.DeltaAngle(directionDegrees, pointAngle);
            return Mathf.Abs(relativeAngle) <= angle * 0.5f;
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
            float clampedInnerRadius = Mathf.Clamp(innerRadius, 0f, radius);
            float halfAngle = angle * 0.5f;
            int sampledArcSegments = Mathf.Max(2, Mathf.CeilToInt(arcSegments * Mathf.Max(angle, 1f) / 360f) + 1);

            if (radius <= Mathf.Epsilon || angle <= Mathf.Epsilon)
            {
                return new[] { ApplySpace(OffsetPoint(Vector3.zero)), ApplySpace(OffsetPoint(DirectionPoint(0f, radius))) };
            }

            if (clampedInnerRadius <= Mathf.Epsilon)
            {
                Vector3[] points = new Vector3[sampledArcSegments + 2];
                points[0] = ApplySpace(OffsetPoint(Vector3.zero));

                for (int i = 0; i < sampledArcSegments; i++)
                {
                    float t = sampledArcSegments == 1 ? 0f : i / (float)(sampledArcSegments - 1);
                    float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
                    points[i + 1] = ApplySpace(OffsetPoint(DirectionPoint(currentAngle, radius)));
                }

                points[points.Length - 1] = ApplySpace(OffsetPoint(Vector3.zero));
                return points;
            }

            Vector3[] ringPoints = new Vector3[(sampledArcSegments * 2) + 1];

            for (int i = 0; i < sampledArcSegments; i++)
            {
                float t = sampledArcSegments == 1 ? 0f : i / (float)(sampledArcSegments - 1);
                float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
                ringPoints[i] = ApplySpace(OffsetPoint(DirectionPoint(currentAngle, clampedInnerRadius)));
            }

            for (int i = 0; i < sampledArcSegments; i++)
            {
                float t = sampledArcSegments == 1 ? 0f : i / (float)(sampledArcSegments - 1);
                float currentAngle = Mathf.Lerp(halfAngle, -halfAngle, t);
                ringPoints[sampledArcSegments + i] = ApplySpace(OffsetPoint(DirectionPoint(currentAngle, radius)));
            }

            ringPoints[ringPoints.Length - 1] = ringPoints[0];
            return ringPoints;
        }

        private Vector3 DirectionPoint(float deltaAngleDegrees, float pointRadius)
        {
            float radians = (directionDegrees + deltaAngleDegrees) * Mathf.Deg2Rad;
            Vector3 rightComponent = PlaneRight * (Mathf.Cos(radians) * visualScale.x);
            Vector3 forwardComponent = PlaneForward * (Mathf.Sin(radians) * visualScale.y);
            return (rightComponent + forwardComponent) * pointRadius;
        }

        private Vector3 OffsetPoint(Vector3 point)
        {
            return point + Vector3.forward * heightOffset;
        }

        private Vector2 CompensateForVisualScale(Vector2 planarDirection)
        {
            float compensatedX = visualScale.x > Mathf.Epsilon ? planarDirection.x / visualScale.x : planarDirection.x;
            float compensatedY = visualScale.y > Mathf.Epsilon ? planarDirection.y / visualScale.y : planarDirection.y;
            return new Vector2(compensatedX, compensatedY);
        }

        private Vector3 ApplySpace(Vector3 point)
        {
            return useLocalSpace ? point : transform.TransformPoint(point);
        }

        private static float ProjectToPlaneAxis(Vector3 vector, Vector3 axis)
        {
            return Vector3.Dot(vector, axis.normalized);
        }
    }
}
