using UnityEngine;

namespace AWE.Synzza.Monocomponents {
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerControllerMonocomponent : MonoBehaviour {
        [SerializeField] private float _speed;

        private Rigidbody _rigidbody;

        private void Awake() {
            var success = TryGetComponent(out _rigidbody);
            Debug.Assert(success, $"Could not retrieve {typeof(Rigidbody).Name} component of {GetType().Name} {gameObject.name}!");
        }

        private void FixedUpdate() {
            var speed = Time.fixedDeltaTime * _speed;
            var delta = new Vector3(speed * Input.GetAxis("Horizontal"), 0f, speed * Input.GetAxis("Vertical"));
            _rigidbody.MovePosition(transform.position + delta);
        }
    }
}
