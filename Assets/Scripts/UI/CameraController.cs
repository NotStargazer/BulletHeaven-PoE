
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Random = UnityEngine.Random;

namespace UI
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector2 _rotationDegrees;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private float _distance;
        [SerializeField] private float _smoothJumpDistance;
        [SerializeField] private float _smoothJumpDuration;

        private TransitionFunction _jumpTransition;
        private Transform _target;
        private Vector3 _originPoint;
        private readonly Plane _plane = new(Vector3.up, 0);

        public Quaternion WorldRotation => Quaternion.Euler(0, _rotationDegrees.y, 0);


        private void Awake()
        {
            _jumpTransition = new TransitionFunction(JumpTick);
        }

        private void LateUpdate()
        {
            if (_camera.enabled && _target)
            {
                if (_jumpTransition.Active)
                {
                    return;
                }

                if (Vector3.Distance(_originPoint, _target.localPosition) >= _smoothJumpDistance)
                {
                    _jumpTransition.StartTransition(_smoothJumpDuration, 0, 1, easeFunc: Easing.OutCubic);
                    return;
                }
                
                LookAtTarget();
            }
            else
            {
                enabled = false;
            }
        }

        private void LookAtTarget()
        {
            var camTransform = _camera.transform;
            _originPoint = _target.localPosition;
            camTransform.localRotation = Quaternion.Euler(_rotationDegrees);
            camTransform.localPosition = _originPoint;
                
            camTransform.Translate(-camTransform.forward * _distance, Space.World);
            camTransform.Translate(_offset, Space.Self);
        }

        private void JumpTick(float t)
        {
            var camTransform = _camera.transform;

            if (Vector3.Distance(camTransform.position, _target.localPosition) >= _smoothJumpDistance)
            {
                _jumpTransition.StartTransition(_smoothJumpDuration, 0, 1, easeFunc: Easing.OutCubic);
            }
                
            camTransform.localRotation = Quaternion.Euler(_rotationDegrees);
            camTransform.position = Vector3.Lerp(_originPoint, _target.localPosition, t);
            camTransform.Translate(-camTransform.forward * _distance);
        }

        private void OnValidate()
        {
            _camera = GetComponent<Camera>();
        }

        public void Attach(Transform target)
        {
            _target = target;
            enabled = true;
            LookAtTarget();
        }

        public Transform Detach()
        {
            var target = _target;
            _target = null;
            enabled = false;
            return target;
        }

        public Vector3? GetWorldFloor(Vector2 value)
        {
            var ray = _camera.ScreenPointToRay(value);
            if (_plane.Raycast(ray, out var d))
            {
                return ray.GetPoint(d);
            }
            
            return null;
        }

        public bool TryGetRandomBorderPosition(out Vector3 pos)
        {
            var rng = Random.Range(-0.25f, 1.25f);
            var side = Random.Range(0, 4);
            var ray = _camera.ViewportPointToRay(side switch
            {
                1 => new Vector2(1.25f, rng),
                2 => new Vector2(rng, -0.25f),
                3 => new Vector2(rng, 1.25f),
                _ => new Vector3(-0.25f, rng),
            });
            
            if (_plane.Raycast(ray, out var d))
            {
                pos = ray.GetPoint(d);
                return true;
            }
            
            pos = Vector3.zero;
            return false;
        }

        public Vector2 GetUIScreenPosition(Vector3 position)
        {
            return RectTransformUtility.WorldToScreenPoint(_camera, position);
        }
    }
}
