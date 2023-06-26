using AWE.Synzza.UnityLayer;
using AWE.Synzza.UnityLayer.Monocomponents;
using UnityEngine;

namespace AWE.Synzza.Demo.UnityLayer.Monocomponents {
    public class DemoNavmeshAgentMonocomponent : NavmeshAgentMonocomponent {
        [Tooltip("Settings for this agent's movement")]
        [SerializeField] private DemoNpcMovementSerializable _movementSettings;

        private DemoNpcMovement _movement = null;

        protected override void Awake() {
            base.Awake();
            _movement = _movementSettings.ToDemoNpcMovement();
        }

        protected override void PickNewDestination() {
            _currentTarget = _movement.PickNewMovementTarget(_currentTarget);
            _currentTargetTransform = _currentTarget is UnitySceneObject currentTargetTransform ? currentTargetTransform.transform : null;

            if (_currentTargetTransform != null) {
                _agent.SetDestination(_currentTargetTransform.position);
            }
        }
    }
}
