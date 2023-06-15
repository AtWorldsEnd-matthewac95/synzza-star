using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace AWE.Synzza.Monocomponents {
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAiMonocomponent : MonoBehaviour {
        [SerializeField] private Transform[] _positionalTargets;
        [SerializeField] private int _validPositionalTargetCount;
        [SerializeField] [Range(0f, 1f)] private float _targetPlayerChance;

        private NavMeshAgent _agent;
        private PlayerControllerMonocomponent _player;
        private Transform _currentTarget;
        private int _positionalTargetSwapIndex;

        private void Awake() {
            bool success = TryGetComponent(out _agent);
            Debug.Assert(success, $"{GetType().Name} {gameObject.name} has no {typeof(NavMeshAgent).Name} component!");

            if (success) {
                _agent.autoBraking = false;
                _agent.autoRepath = false;
                _agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            }
        }

        private void Start() {
            _player = FindObjectOfType<PlayerControllerMonocomponent>();

            if (_player == null) {
                Debug.LogWarning($"{GetType().Name} {gameObject.name} failed to find an instance of {typeof(PlayerControllerMonocomponent).Name}!");
            }

            PickNewDestination();
        }

        private void PickNewDestination() {
            _currentTarget = PickNewTarget();
            _agent.SetDestination(_currentTarget.position);
        }

        private Transform PickNewTarget() {
            if ((_player != null) && (_currentTarget != _player.transform) && (Random.value < _targetPlayerChance)) {
                return _player.transform;
            }

            var index = Random.Range(0, _validPositionalTargetCount);
            var target = _positionalTargets[index];

            var swapIndex = _validPositionalTargetCount + _positionalTargetSwapIndex;
            _positionalTargetSwapIndex = (_positionalTargetSwapIndex + 1) % (_positionalTargets.Length - _validPositionalTargetCount);

            _positionalTargets[index] = _positionalTargets[swapIndex];
            _positionalTargets[swapIndex] = target;

            return target;
        }

        private void Update() {
            if (!_agent.pathPending && (_agent.pathStatus == NavMeshPathStatus.PathInvalid || _agent.remainingDistance < 0.5f)) {
                PickNewDestination();
            }

            if ((_player != null) && (_currentTarget == _player.transform)) {
                if (_agent.pathStatus == NavMeshPathStatus.PathInvalid) {
                    Debug.Log("Path to player invalid");
                    PickNewDestination();
                } else if ((_currentTarget.position - _agent.destination).sqrMagnitude > 1f) {
                    _agent.destination = _currentTarget.position;
                }
            }
        }

        private void PickNewTargetIfCollidingWithCurrent(Collider other) {
            if (other.transform == _currentTarget) {
                Debug.Log("Collision caused new path");
                PickNewDestination();
            }
        }

        private void OnTriggerEnter(Collider other) {
            PickNewTargetIfCollidingWithCurrent(other);
        }

        private void OnCollisionEnter(Collision collision) {
            PickNewTargetIfCollidingWithCurrent(collision.collider);
        }
    }
}
