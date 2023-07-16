using UnityEngine;
using UnityEngine.AI;

namespace AWE.Synzza.UnityLayer {
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class NavmeshAgentMonocomponent : MonoBehaviour {
        protected NavMeshAgent _agent;

        protected IWorldObject _currentTarget = null;
        protected Transform _currentTargetTransform = null;

        protected virtual void Awake() {
            _agent = GetComponent<NavMeshAgent>();
        }

        protected virtual void Start() {
            PickNewDestination();
        }

        protected abstract void PickNewDestination();

        protected virtual void Update() {
            if (!_agent.pathPending && (_agent.pathStatus == NavMeshPathStatus.PathInvalid || _agent.remainingDistance < 0.5f)) {
                PickNewDestination();
            }

            if (_currentTarget?.IsMobile ?? false) {
                if (_agent.pathStatus == NavMeshPathStatus.PathInvalid) {
                    Debug.Log("Path to target invalid");
                    PickNewDestination();
                } else if ((_currentTargetTransform.position - _agent.destination).sqrMagnitude > 1f) {
                    _agent.destination = _currentTargetTransform.position;
                }
            }
        }
    }
}
