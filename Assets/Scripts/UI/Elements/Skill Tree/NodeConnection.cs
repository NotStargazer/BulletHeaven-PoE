using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace UI.Elements.Skill_Tree
{
    [RequireComponent(typeof(RawImage))]
    public class NodeConnection : MonoBehaviour
    {
        private static readonly int WIDTH_RATIO = Shader.PropertyToID("_WidthRatio");
        private static readonly int RADIUS = Shader.PropertyToID("_Radius");
        private static readonly int CLAMP = Shader.PropertyToID("_Clamp");
        private static readonly int RANGE = Shader.PropertyToID("_Range");

        [SerializeField] private RectTransform _transform;
        [SerializeField] private SkillNode _pointA;
        [SerializeField] private SkillNode _pointB;
        [SerializeField] private RawImage _image;
        [SerializeField] private Material _material;
        [Range(-1, 1)] [SerializeField] private float _radius;
        [SerializeField] private float _width;
        [SerializeField] private AnimationCurve _clampCurve;

        private RectTransform PointA => (RectTransform)_pointA.transform;
        private RectTransform PointB => (RectTransform)_pointB.transform;
        
        private Vector4 _currentRange;
        private Material _materialInstance;
        public Vector3 Middle => (PointA.position + PointB.position) / 2;

        public float Length => Vector3.Distance(PointA.position, PointB.position);
        
        private void Awake()
        {
            _pointA.OnStateUpdated += state => StateUpdated(state, false);
            _pointB.OnStateUpdated += state => StateUpdated(state, true);
            _currentRange = new Vector4(0.5f, 0f, 0, 0);
            _materialInstance = new Material(_material);
            _image.material = _materialInstance;
            CalculateConnection();
            UpdateMaterial();
        }

        public void Connect(SkillNode a, SkillNode b)
        {
            _pointA = a;
            _pointB = b;
            _materialInstance = new Material(_material);
            _image.material = _materialInstance;
            CalculateConnection();
        }

        private void StateUpdated(NodeState state, bool opposite)
        {
            var range = _currentRange;
            if (!opposite)
            {
                range.z = state switch
                {
                    NodeState.Selected => 0.5f,
                    NodeState.Allocated => 1,
                    _ => 0
                };
            }
            else
            {
                range.w = state switch
                {
                    NodeState.Selected => 0.5f,
                    NodeState.Allocated => 1,
                    _ => 0
                };
            }

            if (range is { z: > 0.1f, w: > 0.1f })
            {
                range.x = 1;
                range.y = 0;
            }
            else
            {
                range.x = 0.25f;
                range.y = 0.25f;
            }
            _currentRange = range;
            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            _materialInstance.SetVector(RANGE, _currentRange);
        }

        private void OnValidate()
        {
            if (!_transform)
            {
                _transform = GetComponent<RectTransform>();
            }

            if (!_image)
            {
                _image = GetComponent<RawImage>();
            }
            else if (_material && !_materialInstance)
            {
                _materialInstance = new Material(_material);
                _image.material = _materialInstance;
            }
            else if (_materialInstance)
            {
                _image.material = _materialInstance;
            }

            if (!_pointA || !_pointB || _width == 0)
            {
                return;
            }

            CalculateConnection();
        }

        private void CalculateConnection()
        {
            var middle = Middle;
            var length = Length;
            var direction = PointB.position - PointA.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var radius = Easing.OutCirc(Mathf.Abs(_radius)) * Mathf.Sign(_radius);
            _transform.position = middle;
            _transform.sizeDelta = new Vector2(length + _width * Mathf.Abs(radius), _width + length / 2 * Mathf.Abs(radius));
            _transform.rotation = Quaternion.Euler(0, 0, angle);
            _transform.position += (_width / 2 * Mathf.Abs(radius) * Mathf.Sign(_radius) + length / 2 * radius / 2) * _transform.up;
            
            _materialInstance.SetFloat(WIDTH_RATIO, _width / _transform.sizeDelta.y);
            _materialInstance.SetFloat(RADIUS, radius);
            _materialInstance.SetFloat(CLAMP, _clampCurve.Evaluate(Mathf.Abs(_radius)));
            _materialInstance.SetVector(RANGE, new Vector4(0.5f, 0f, 1f, 1f));
        }
    }
}
