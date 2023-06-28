using AWE.Synzza.UnityLayer;
using AWE.Synzza.UnityLayer.Monocomponents;
using UnityEngine;

namespace AWE.Synzza.Demo.UnityLayer.Monocomponents {
    public class DemoEnemyNavAgentMonocomponent : DemoBattlerNavAgentMonocomponent {
        [Tooltip("Settings for this agent's movement")]
        [SerializeField] private DemoEnemyMovementSerializable _movementSettings;
        [Tooltip("Needed to make this enemy attack and block")]
        [SerializeField] private EnemyBattlerMonocomponent _battlerMonocomponent;

        private DemoEnemyMovement _movement = null;

        public override Battler Battler => _battlerMonocomponent.Battler;
        public override IBattlerMonocomponent BattlerMono => _battlerMonocomponent;

        protected override void Awake() {
            if (_battlerMonocomponent == null) {
                Debug.LogError($"{GetType().Name} {gameObject.name} has no battler to reference!");
                Destroy(gameObject);
            } else {
                base.Awake();
                _movement = _movementSettings.ToDemoEnemyMovement();
                _agent.speed = _movement.Speed;
            }
        }

        protected override void Start() {
            Battler.CurrentContinuousState = BattlerContinuousState.OpportuneAttack;
            base.Start();
        }

        protected override void OnBattlerBlockingStatusChanged(bool isBlockingNow) {
            _agent.speed = isBlockingNow ? _movement.Speed / 4f : _movement.Speed;
        }

        protected override void OnBattlerWindDown() {
            base.OnBattlerWindDown();
            PickNewDestination();
        }

        internal override void PickNewDestination() {
            _currentTarget = _movement.PickNewMovementTarget(_currentTarget);
            _currentTargetTransform = _currentTarget is UnitySceneObject currentTargetTransform ? currentTargetTransform.transform : null;

            if (_currentTargetTransform != null) {
                _agent.SetDestination(_currentTargetTransform.position);

                if (_currentTargetTransform.gameObject.TryGetComponent(out PlayerBattlerMonocomponent targetBattler)) {
                    TargetBattler = targetBattler;
                }
            }

            UpdateContinuousState();
        }

        protected override void UpdateContinuousState() {
            Battler.CurrentContinuousState = _currentTarget?.IsMobile ?? false ? BattlerContinuousState.AutoAttack : BattlerContinuousState.OpportuneAttack;
        }
    }
}
