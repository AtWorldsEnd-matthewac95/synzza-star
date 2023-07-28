using AWE.Synzza.Demo;
using AWE.Synzza.Demo.UnityLayer;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Example {
    public class EnemyNavAgentMonocomponent : BattlerNavAgentMonocomponent {
        [Tooltip("Settings for this agent's movement")]
        [SerializeField] private DemoEnemyMovementSerializable _movementSettings;

        private DemoEnemyMovement _movement = null;

        protected override bool IsAgentPickingNewTargetsBasedOnProximity => true;

        protected override void Awake() {
            base.Awake();
            //WorldObject.Battler.Status.OnSkillWindDown += OnBattlerSkillWindDown;
            _movement = _movementSettings.ToDemoEnemyMovement();
            _agent.speed = _movement.Speed;
        }

        protected override void OnBattlerBlockStatusChanged(bool isBlockingNow) {
            _agent.speed = isBlockingNow ? _movement.Speed / 4f : _movement.Speed;
        }
        /*

        protected void OnBattlerSkillWindDown() {
            PickNewDestination();
        }
        */

        protected override void PickNewDestination() {
            _currentTarget = _movement.PickNewMovementTarget(_currentTarget);
            _currentTargetTransform = _currentTarget is WorldObject currentTargetTransform ? currentTargetTransform.GetImpl<UnityWorldObject>().transform : null;

            if (_currentTargetTransform != null) {
                _agent.SetDestination(_currentTargetTransform.position);

                if (_currentTargetTransform.gameObject.TryGetComponent(out BattlerNavAgentMonocomponent targetBattler)) {
                    WorldObject.Battler.SetTargetBattler(targetBattler.WorldObject);
                }
            }

            UpdateContinuousState();
        }

        protected void UpdateContinuousState() {
            WorldObject.Battler.CurrentMeleeRules = _currentTarget?.IsMobile ?? false ? BattlerMeleeRules.AutoAttack : BattlerMeleeRules.OpportuneAttack;
        }

        protected override void OnAttacked() => PickNewDestination();
    }
}
