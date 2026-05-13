using UnityEngine;
using UnityEngine.UI;

namespace Rimus.Scripts.Tools.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIConnectionGraphic : Graphic
    {
        public enum ConnectionMode
        {
            Solid,
            Dashed,
            Dotted
        }

        [Header("Connection")]
        [SerializeField] private RectTransform start;
        [SerializeField] private RectTransform end;

        [Header("Shape")]
        [SerializeField] private float thickness = 6f;
        [SerializeField] private ConnectionMode mode = ConnectionMode.Solid;

        [Header("Dashed / Dotted")]
        [SerializeField] private float dashLength = 24f;
        [SerializeField] private float gapLength = 12f;

        [Header("Shader Support")]
        [Tooltip("UV.x goes along the line. Higher values repeat shader texture/pattern more often.")]
        [SerializeField] private float uvScale = 1f;

        [Tooltip("Optional material used only by this connection instance.")]
        [SerializeField] private Material connectionMaterial;

        [Tooltip("Animates _FlowOffset on the material.")]
        [SerializeField] private bool animateFlow = false;

        [SerializeField] private float flowSpeed = 1f;

        private Material runtimeMaterial;

        private Vector3 lastStartPosition;
        private Vector3 lastEndPosition;
        private Vector2 lastSize;

        protected override void Awake()
        {
            base.Awake();
            SetupMaterial();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetupMaterial();
            SetVerticesDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (Application.isPlaying && runtimeMaterial != null)
            {
                Destroy(runtimeMaterial);
            }
            else if (runtimeMaterial != null)
            {
                DestroyImmediate(runtimeMaterial);
            }

            runtimeMaterial = null;
        }

        private void SetupMaterial()
        {
            if (connectionMaterial == null)
                return;

            if (runtimeMaterial != null)
                return;

            runtimeMaterial = Instantiate(connectionMaterial);
            material = runtimeMaterial;
        }

        private void Update()
        {
            if (start == null || end == null)
                return;

            bool geometryChanged =
                lastStartPosition != start.position ||
                lastEndPosition != end.position ||
                lastSize != rectTransform.rect.size;

            if (geometryChanged)
            {
                lastStartPosition = start.position;
                lastEndPosition = end.position;
                lastSize = rectTransform.rect.size;
                SetVerticesDirty();
            }

            if (animateFlow && runtimeMaterial != null)
            {
                runtimeMaterial.SetFloat("_FlowOffset", Time.time * flowSpeed);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (start == null || end == null)
                return;

            Vector2 startPos = GetLocalPoint(start);
            Vector2 endPos = GetLocalPoint(end);

            Vector2 line = endPos - startPos;
            float distance = line.magnitude;

            if (distance <= 0.01f)
                return;

            switch (mode)
            {
                case ConnectionMode.Solid:
                    DrawSolidLine(vh, startPos, endPos, distance);
                    break;

                case ConnectionMode.Dashed:
                    DrawDashedLine(vh, startPos, endPos, distance);
                    break;

                case ConnectionMode.Dotted:
                    DrawDottedLine(vh, startPos, endPos, distance);
                    break;
            }
        }

        private void DrawSolidLine(VertexHelper vh, Vector2 startPos, Vector2 endPos, float distance)
        {
            float uvEnd = distance / Mathf.Max(1f, uvScale);
            AddLineSegment(vh, startPos, endPos, 0f, uvEnd);
        }

        private void DrawDashedLine(VertexHelper vh, Vector2 startPos, Vector2 endPos, float distance)
        {
            Vector2 direction = (endPos - startPos).normalized;

            float step = dashLength + gapLength;
            float currentDistance = 0f;

            while (currentDistance < distance)
            {
                float segmentStartDistance = currentDistance;
                float segmentEndDistance = Mathf.Min(currentDistance + dashLength, distance);

                Vector2 segmentStart = startPos + direction * segmentStartDistance;
                Vector2 segmentEnd = startPos + direction * segmentEndDistance;

                float uvStart = segmentStartDistance / Mathf.Max(1f, uvScale);
                float uvEnd = segmentEndDistance / Mathf.Max(1f, uvScale);

                AddLineSegment(vh, segmentStart, segmentEnd, uvStart, uvEnd);

                currentDistance += step;
            }
        }

        private void DrawDottedLine(VertexHelper vh, Vector2 startPos, Vector2 endPos, float distance)
        {
            Vector2 direction = (endPos - startPos).normalized;

            float dotLength = thickness;
            float step = dotLength + gapLength;
            float currentDistance = 0f;

            while (currentDistance < distance)
            {
                float segmentStartDistance = currentDistance;
                float segmentEndDistance = Mathf.Min(currentDistance + dotLength, distance);

                Vector2 segmentStart = startPos + direction * segmentStartDistance;
                Vector2 segmentEnd = startPos + direction * segmentEndDistance;

                float uvStart = segmentStartDistance / Mathf.Max(1f, uvScale);
                float uvEnd = segmentEndDistance / Mathf.Max(1f, uvScale);

                AddLineSegment(vh, segmentStart, segmentEnd, uvStart, uvEnd);

                currentDistance += step;
            }
        }

        private void AddLineSegment(
            VertexHelper vh,
            Vector2 startPoint,
            Vector2 endPoint,
            float uvStart,
            float uvEnd)
        {
            Vector2 segment = endPoint - startPoint;

            if (segment.sqrMagnitude <= 0.001f)
                return;

            Vector2 direction = segment.normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * thickness * 0.5f;

            int index = vh.currentVertCount;

            Color32 vertexColor = color;

            vh.AddVert(startPoint - normal, vertexColor, new Vector2(uvStart, 0f));
            vh.AddVert(startPoint + normal, vertexColor, new Vector2(uvStart, 1f));
            vh.AddVert(endPoint + normal, vertexColor, new Vector2(uvEnd, 1f));
            vh.AddVert(endPoint - normal, vertexColor, new Vector2(uvEnd, 0f));

            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index + 2, index + 3, index);
        }

        private Vector2 GetLocalPoint(RectTransform target)
        {
            Camera camera = null;

            Canvas canvas = canvasRenderer.GetComponentInParent<Canvas>();

            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                camera = canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                RectTransformUtility.WorldToScreenPoint(camera, target.position),
                camera,
                out Vector2 localPoint
            );

            return localPoint;
        }

        public void SetNodes(RectTransform newStart, RectTransform newEnd)
        {
            start = newStart;
            end = newEnd;
            SetVerticesDirty();
        }

        public void SetMode(ConnectionMode newMode)
        {
            mode = newMode;
            SetVerticesDirty();
        }

        public void SetThickness(float newThickness)
        {
            thickness = Mathf.Max(0.1f, newThickness);
            SetVerticesDirty();
        }

        public void SetDashSettings(float newDashLength, float newGapLength)
        {
            dashLength = Mathf.Max(0.1f, newDashLength);
            gapLength = Mathf.Max(0f, newGapLength);
            SetVerticesDirty();
        }

        public static UIConnectionGraphic Create(Transform parent)
        {
            GameObject go = new GameObject("UI Connection", typeof(RectTransform), typeof(UIConnectionGraphic));
            go.transform.SetParent(parent, false);
            return go.GetComponent<UIConnectionGraphic>();
        }
    }
}