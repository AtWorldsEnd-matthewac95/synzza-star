﻿using UnityEngine;
using UnityEngine.AI;

namespace AWE.Synzza.UnityLayer.Monocomponents {
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class NavmeshAgentMonocomponent : MonoBehaviour {
        protected NavMeshAgent _agent;

        protected ISceneObject _currentTarget = null;
        protected Transform _currentTargetTransform = null;

        protected virtual void Awake() {
            bool success = TryGetComponent(out _agent);
            Debug.Assert(success, $"{GetType().Name} {gameObject.name} has no {typeof(NavMeshAgent).Name} component!");

            if (success) {
                _agent.autoBraking = false;
                _agent.autoRepath = false;
                _agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            }
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

        private void PickNewTargetIfCollidingWithCurrent(Collider other) {
            if (other.transform == _currentTargetTransform) {
                Debug.Log("Collision caused new path");
                PickNewDestination();
            }
        }

        protected virtual void OnTriggerEnter(Collider other) {
            PickNewTargetIfCollidingWithCurrent(other);
        }

        protected virtual void OnCollisionEnter(Collision collision) {
            PickNewTargetIfCollidingWithCurrent(collision.collider);
        }

        public virtual void PauseAgent() {
            _agent.isStopped = true;
        }

        public virtual void ResumeAgent() {
            _agent.isStopped = false;
        }
    }
}