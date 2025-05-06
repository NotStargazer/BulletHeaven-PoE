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

        [SerializeField] private RectTransform _transform;
        [SerializeField] private RectTransform _pointA;
        [SerializeField] private RectTransform _pointB;
        [SerializeField] private RawImage _image;
        [SerializeField] private Material _material;
        [Range(-1, 1)] [SerializeField] private float _radius;
        [SerializeField] private float _width;
        [SerializeField] private Vector4 _range;

        private Material _materialInstance;
        public Vector3 Middle => (_pointA.position + _pointB.position) / 2;

        public Vector2 Glow
        {
            get => new(_range.x, _range.y);
            set
            {
                var temp = _range;
                temp.x = value.x;
                temp.y = value.y;
                _range = temp;
            }
        }
        public Vector2 Strength
        {
            get => new(_range.z, _range.w);
            set
            {
                var temp = _range;
                temp.z = value.x;
                temp.w = value.y;
                _range = temp;
            }
        }

        public float Length => Vector3.Distance(_pointA.position, _pointB.position);
        public float Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                CalculateConnection();
            }
        }



        private void Awake()
        {
            _materialInstance = new Material(_material);
            _image.material = _materialInstance;
        }

        public void Connect(SkillNode a, SkillNode b)
        {
            _pointA = (RectTransform)a.transform;
            _pointB = (RectTransform)b.transform;
            _materialInstance = new Material(_material);
            _image.material = _materialInstance;
            CalculateConnection();
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
            var direction = _pointB.position - _pointA.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var radius = Easing.OutCirc(Mathf.Abs(_radius)) * Mathf.Sign(_radius);
            _transform.position = middle;
            _transform.sizeDelta = new Vector2(length + _width * Mathf.Abs(radius), _width + length / 2 * Mathf.Abs(radius));
            _transform.rotation = Quaternion.Euler(0, 0, angle);
            _transform.position += (_width / 2 * Mathf.Abs(radius) * Mathf.Sign(_radius) + length / 2 * radius / 2) * _transform.up;
            
            _materialInstance.SetFloat(WIDTH_RATIO, _width / _transform.sizeDelta.y);
            _materialInstance.SetFloat(RADIUS, radius);
            _materialInstance.SetVector("_Range", _range);
        }
    }
}
