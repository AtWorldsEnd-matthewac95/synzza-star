using AWE.Synzza.Demo.UnityLayer.Monocomponents;
using AWE.Synzza.UnityLayer.Monocomponents.UI;
using AWE.Synzza.UnityLayer.Scrib;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Monocomponents {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(DemoPlayerMovementMonocomponent))]
    public class PlayerBattlerMonocomponent : DemoBattlerNavAgentMonocomponent, IBattlerMonocomponent {
        [SerializeField] private float _speed;
        [SerializeField] private SkillScrib _attack;
        [SerializeField] private SkillScrib _block;
        [SerializeField] private BattlerScrib _battler;
        [SerializeField] private BattlerContinuousState _continuousState;
        [SerializeField] private BattlerStatusUI _statusText;

        [SerializeField] private bool _isCurrentlyNavmeshControlled = false;

        private Rigidbody _rigidbody;
        private DemoPlayerMovementMonocomponent _movement;

        public Battler PlayerBattler { get; private set; }
        public override Battler Battler => PlayerBattler;
        public override IBattlerMonocomponent BattlerMono => this;
        public string DisplayName => PlayerBattler.DisplayName;
        public bool IsCurrentlyNavmeshControlled => _isCurrentlyNavmeshControlled;

        protected override void Awake() {
            base.Awake();
            _agent.speed = _speed;

            PlayerBattler = _battler.ToBattler(BattlerTeam.Players);
            PlayerBattler.CurrentContinuousState = BattlerContinuousState.AutoCounter;

            _rigidbody = GetComponent<Rigidbody>();
            _movement = GetComponent<DemoPlayerMovementMonocomponent>();

            SetIsCurrentlyNavmeshControlled(false, isAllowingEarlyReturn: false);

            if (_statusText != null) { _statusText.BattlerMono = this; }
        }

        protected override void Start() {
            PlayerBattler.CurrentContinuousState = BattlerContinuousState.AutoBlock;
            base.Start();
        }

        protected override void OnBattlerBlockingStatusChanged(bool isBlockingNow) {
            _agent.speed = isBlockingNow ? _speed / 4f : _speed;
        }

        internal override void PickNewDestination() {
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
                /*
                var speed = Time.deltaTime * _speed;
                var delta = new Vector3(speed * Input.GetAxis("Horizontal"), 0f, speed * Input.GetAxis("Vertical"));
                _agent.Move(delta);
                */
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
        }

        private void Debug_CheckForContinuousStateChange() {
            UpdateContinuousState();
        }

        protected override void UpdateContinuousState() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                SetIsCurrentlyNavmeshControlled(true);
                Battler.CurrentContinuousState = BattlerContinuousState.AutoAttack;
                Debug.Log("Setting to Auto Attack.");
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                SetIsCurrentlyNavmeshControlled(true);
                Battler.CurrentContinuousState = BattlerContinuousState.AutoCounter;
                Debug.Log("Setting to Auto Counter.");
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                SetIsCurrentlyNavmeshControlled(true);
                Battler.CurrentContinuousState = BattlerContinuousState.OpportuneAttack;
                Debug.Log("Setting to Opportune Attack.");
            }
        }

        protected override void ReactToAttackHitbox(Collider collider) {
            base.ReactToAttackHitbox(collider);

            if (!_isCurrentlyNavmeshControlled && (Battler?.CurrentStatus ?? BattlerStatusState.OK) == BattlerStatusState.Staggered) {
                SetIsCurrentlyNavmeshControlled(true);
            }
        }
    }
}
