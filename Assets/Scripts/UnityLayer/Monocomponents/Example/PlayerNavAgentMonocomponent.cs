using AWE.Synzza.Demo.UnityLayer;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Example {
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerNavAgentMonocomponent : BattlerNavAgentMonocomponent {
        [SerializeField] private float _speed;
        [SerializeField] private BattlerMeleeRules _continuousState;
        [SerializeField] private PlayerBattleInputUI _battleInput;

        [SerializeField] private bool _isCurrentlyNavmeshControlled = false;

        protected override bool IsAgentPickingNewTargetsBasedOnProximity => false;

        private Rigidbody _rigidbody;

        protected override void Awake() {
            base.Awake();
            _agent.speed = _speed;

            _rigidbody = GetComponent<Rigidbody>();
            _battleInput.OnPlayerSelectedTargetBattler += OnPlayerSelectedTargetBattler;
            _battleInput.AssignPlayer(WorldObject);

            SetIsCurrentlyNavmeshControlled(false, isAllowingEarlyReturn: false);
        }

        protected override void OnBattlerBlockStatusChanged(bool isBlockingNow) {
            _agent.speed = isBlockingNow ? _speed / 4f : _speed;
        }

        protected override void PickNewDestination() {
            if (!_battleInput.IsSelecting) {
                _battleInput.BeginSelection();
            }
        }

        protected virtual void OnPlayerSelectedTargetBattler(BattlerWorldObject enemy) {
            if (_battleInput.IsSelecting) {
                var enemyTransform = enemy.GetImpl<UnityWorldObject>();
                _currentTarget = enemy;
                _currentTargetTransform = enemyTransform.transform;
                WorldObject.Battler.SetTargetBattler(enemy);
                _battleInput.EndSelection();
            }
        }

        protected override void Update() {
            Debug_CheckForControlSwap();
            Debug_CheckForContinuousStateChange();

            if (_isCurrentlyNavmeshControlled) {
                base.Update();
            }
        }

        private void FixedUpdate() {
            if (!_isCurrentlyNavmeshControlled) {
                var speed = Time.fixedDeltaTime * _speed;
                var delta = new Vector3(speed * Input.GetAxis("Horizontal"), 0f, speed * Input.GetAxis("Vertical"));
                _rigidbody.MovePosition(transform.position + delta);
            }
        }

        private void Debug_CheckForControlSwap() {
            if (Input.GetKeyDown(KeyCode.N)) {
                ToggleIsCurrentlyNavmeshControlled();
            }
        }

        public void ToggleIsCurrentlyNavmeshControlled() {
            SetIsCurrentlyNavmeshControlled(!_isCurrentlyNavmeshControlled);
        }

        public void SetIsCurrentlyNavmeshControlled(bool value) {
            SetIsCurrentlyNavmeshControlled(value, isAllowingEarlyReturn: true);
        }

        private void SetIsCurrentlyNavmeshControlled(bool value, bool isAllowingEarlyReturn) {
            if (isAllowingEarlyReturn && _isCurrentlyNavmeshControlled == value) {
                return;
            }

            _isCurrentlyNavmeshControlled = value;

            if (_isCurrentlyNavmeshControlled) {
                _agent.enabled = true;
                _agent.velocity = _rigidbody.velocity;
                _rigidbody.velocity = Vector3.zero;
            } else {
                _rigidbody.velocity = _agent.velocity;
                _agent.velocity = Vector3.zero;
                _agent.enabled = false;
            }

            SetRigidbodyEnabled(!_isCurrentlyNavmeshControlled);
        }

        private void SetRigidbodyEnabled(bool isEnabled) {
            _rigidbody.useGravity = isEnabled;
            _rigidbody.constraints = isEnabled ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeAll;
        }

        private void Debug_CheckForContinuousStateChange() {
            UpdateContinuousState();
        }

        protected void UpdateContinuousState() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                SetIsCurrentlyNavmeshControlled(true);
                WorldObject.Battler.CurrentMeleeRules = BattlerMeleeRules.AutoAttack;
                Debug.Log("Setting to Auto Attack.");
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                SetIsCurrentlyNavmeshControlled(true);
                WorldObject.Battler.CurrentMeleeRules = BattlerMeleeRules.AutoCounter;
                Debug.Log("Setting to Auto Counter.");
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                SetIsCurrentlyNavmeshControlled(true);
                WorldObject.Battler.CurrentMeleeRules = BattlerMeleeRules.OpportuneAttack;
                Debug.Log("Setting to Opportune Attack.");
            }
        }

        protected override void ReactToAttackHitbox(Collider collider) {
            base.ReactToAttackHitbox(collider);

            if (!_isCurrentlyNavmeshControlled && (WorldObject.Battler?.Status.Current ?? BattlerStatusState.OK) == BattlerStatusState.Staggered) {
                SetIsCurrentlyNavmeshControlled(true);
            }
        }
    }
}
