using AWE.Synzza.UnityLayer;
using UnityEngine;

namespace AWE.Synzza.Demo.UnityLayer {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(DemoPlayerMovementMonocomponent))]
    [RequireComponent(typeof(BattlerMonocomponent))]
    public class DemoPlayerControllerMonocomponent : DemoBattlerNavAgentMonocomponent {
        [SerializeField] private float _speed;
        [SerializeField] private BattlerMeleeRules _continuousState;

        [SerializeField] private bool _isCurrentlyNavmeshControlled = false;

        private Rigidbody _rigidbody;
        private DemoPlayerMovementMonocomponent _movement;
        private BattlerMonocomponent _battler;

        public Battler PlayerBattler { get; private set; }
        public override Battler Battler => PlayerBattler;
        public override BattlerMonocomponent BattlerMono => _battler;
        public string DisplayName => PlayerBattler.DisplayName;
        public bool IsCurrentlyNavmeshControlled => _isCurrentlyNavmeshControlled;

        protected override void Awake() {
            base.Awake();
            _agent.speed = _speed;

            _rigidbody = GetComponent<Rigidbody>();
            _movement = GetComponent<DemoPlayerMovementMonocomponent>();
            _battler = GetComponent<BattlerMonocomponent>();
            PlayerBattler = _battler.Battler;
            var name = PlayerBattler?.DisplayName ?? "null";
            Debug.Log($"Player is \"{name}\"");

            SetIsCurrentlyNavmeshControlled(false, isAllowingEarlyReturn: false);
        }

        protected override void OnBattlerBlockingStatusChanged(bool isBlockingNow) {
            _agent.speed = isBlockingNow ? _speed / 4f : _speed;
        }

        protected override void PickNewDestination() {
            var enemy = _movement.GetNextEnemy();
            _currentTarget = new UnitySceneObject(enemy.transform);
            _currentTargetTransform = enemy.transform;
            TargetBattler = enemy;
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

        protected override void UpdateContinuousState() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                SetIsCurrentlyNavmeshControlled(true);
                Battler.CurrentMeleeRules = BattlerMeleeRules.AutoAttack;
                Debug.Log("Setting to Auto Attack.");
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                SetIsCurrentlyNavmeshControlled(true);
                Battler.CurrentMeleeRules = BattlerMeleeRules.AutoCounter;
                Debug.Log("Setting to Auto Counter.");
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                SetIsCurrentlyNavmeshControlled(true);
                Battler.CurrentMeleeRules = BattlerMeleeRules.OpportuneAttack;
                Debug.Log("Setting to Opportune Attack.");
            }
        }

        protected override void ReactToAttackHitbox(Collider collider) {
            base.ReactToAttackHitbox(collider);

            if (!_isCurrentlyNavmeshControlled && (Battler?.Status.Current ?? BattlerStatusState.OK) == BattlerStatusState.Staggered) {
                SetIsCurrentlyNavmeshControlled(true);
            }
        }
    }
}
