using AWE.Synzza.UnityLayer.Monocomponents;
using AWE.Synzza.UnityLayer.Scrib;
using System.Collections;
using UnityEngine;

namespace AWE.Synzza.Demo.UnityLayer.Monocomponents {
    public abstract class DemoBattlerNavAgentMonocomponent : NavmeshAgentMonocomponent {
        [SerializeField] protected SkillScrib _blockSkill;
        [SerializeField] protected SkillScrib _attackSkill;
        [SerializeField] protected BattlerStaggerProfileScrib _staggerProfile;
        [SerializeField] protected float _invulnerabilityDurationSeconds;

        protected SkillScrib _currentSkill = null;
        protected SkillEffectCoroutine _runningSkill = null;
        private bool _isInvulnerable = false;

        public abstract Battler Battler { get; }
        public abstract IBattlerMonocomponent BattlerMono { get; }

        private IBattlerMonocomponent _targetBattler;
        protected IBattlerMonocomponent TargetBattler {
            get => _targetBattler;
            set {
                _targetBattler = value;
                var targetBattlerName = _targetBattler == null ? "null" : _targetBattler.transform.gameObject.name;
                Debug.Log($"{GetType().Name} \"{gameObject.name}\" has changed their target to \"{targetBattlerName}\".");
            }
        }

        protected override void Awake() {
            _currentSkill = _attackSkill;
            base.Awake();
        }

        protected override void Start() {
            base.Start();

            Battler.OnCancelEffects += OnBattlerCancelEffects;
            Battler.OnUpdateContinuousState += OnBattlerUpdateContinuousState;
            Battler.OnWindDown += OnBattlerWindDown;
            Battler.OnBlockStatusChanged += OnBattlerBlockingStatusChanged;
            Battler.OnStationaryStatusChanged += OnBattlerStationaryStatusChanged;
        }

        protected abstract void OnBattlerBlockingStatusChanged(bool isBlockingNow);

        private void OnBattlerStationaryStatusChanged(bool isNowStationary) {
            if (_agent.isActiveAndEnabled) {
                _agent.isStopped = isNowStationary;

                if (_agent.isStopped) {
                    _agent.velocity = Vector3.zero;
                }
            }
        }

        private void OnBattlerCancelEffects() {
            _currentSkill.Effect.OnEffectInterrupt(BattlerMono, _runningSkill?.State);

            if (_runningSkill != null) {
                StopCoroutine(_runningSkill.Coroutine);
            }

            _runningSkill = null;

            if (Battler.CurrentContinuousState == BattlerContinuousState.AutoCounter) {
                _currentSkill = _blockSkill;
            }

            if (Battler.CurrentStatus == BattlerStatusState.Staggered) {
                StopAllCoroutines();
                StartCoroutine(CreateStaggerCoroutine(_staggerProfile == null ? 0f : _staggerProfile.DurationSeconds));
            }
        }

        protected virtual void OnBattlerWindDown() {
            StartCoroutine(CreateWindDownCoroutine(_runningSkill.State.WindDownDuration));
        }

        private void OnBattlerUpdateContinuousState(bool isSameState, BattlerContinuousState newState) {
            if (isSameState) {
                return;
            }

            if (_runningSkill?.IsInterruptible ?? false) {
                _currentSkill.Effect.OnEffectInterrupt(BattlerMono, _runningSkill.State);
            }

            switch (newState) {

                case BattlerContinuousState.AutoBlock:
                case BattlerContinuousState.AutoCounter:
                    _currentSkill = _blockSkill;
                    break;

                case BattlerContinuousState.AutoAttack:
                case BattlerContinuousState.OpportuneAttack:
                    _currentSkill = _attackSkill;
                    break;

            }
        }

        protected abstract void UpdateContinuousState();

        private IEnumerator CreateSkillCoroutine() {
            _runningSkill = _currentSkill.Effect.ActivateEffect(BattlerMono, _targetBattler, _currentSkill.WindDownDurationSeconds);
            Battler.ApplyStatusState(BattlerStatusState.SkillWindUp);

            if (_runningSkill.Coroutine != null) {
                Debug.Log($"{GetType().Name} \"{gameObject.name}\" created the skill successfully.");

                yield return new WaitForSeconds(_currentSkill.WindUpDurationSeconds);
                Battler.ApplyStatusState(BattlerStatusState.SkillEffect);
                StartCoroutine(_runningSkill.Coroutine);
            } else {
                Debug.Log($"{GetType().Name} \"{gameObject.name}\" failed to create the skill.");

                Battler.RemoveStatusState(BattlerStatusState.SkillWindUp);
                yield break;
            }
        }

        private IEnumerator CreateWindDownCoroutine(float windDownDurationSeconds) {
            if (Battler.CurrentContinuousState == BattlerContinuousState.AutoCounter) {
                _currentSkill = _blockSkill;
            }
            yield return new WaitForSeconds(windDownDurationSeconds);
            Battler.RemoveStatusState(BattlerStatusState.SkillWindDown);
            _runningSkill = null;
        }

        private IEnumerator CreateStaggerCoroutine(float staggerDurationSeconds) {
            yield return new WaitForSeconds(staggerDurationSeconds);
            Battler.RemoveStatusState(BattlerStatusState.Staggered);
        }

        protected override void Update() {
            base.Update();

            if (_targetBattler == null) {
                return;
            }

            bool isOpportune = Battler.CurrentContinuousState != BattlerContinuousState.OpportuneAttack || _targetBattler.Battler.IsVulnerable;

            if (isOpportune && _currentSkill.Effect.IsEffectActivatable(BattlerMono, _targetBattler)) {
                StartCoroutine(CreateSkillCoroutine());
            }
        }

        protected override void OnCollisionEnter(Collision collision) {
            base.OnCollisionEnter(collision);
            ReactToAttackHitbox(collision.collider);
        }

        protected override void OnTriggerEnter(Collider other) {
            base.OnTriggerEnter(other);
            ReactToAttackHitbox(other);
        }

        protected virtual void ReactToAttackHitbox(Collider collider) {
            if (_isInvulnerable || !collider.CompareTag("Attacks") || !collider.gameObject.TryGetComponent(out SkillHitboxColliderMonocomponent colliderMono) || !colliderMono.IsInitialized) {
                return;
            }

            var sourceBattler = colliderMono.Parent.SourceBattler;

            if (sourceBattler.Battler.Team == Battler.Team) {
                return;
            }

            if (sourceBattler.transform.gameObject.TryGetComponent(out NavmeshAgentMonocomponent otherNavAgent)) {
                otherNavAgent.PickNewDestination();
            }
            PickNewDestination();

            if (Battler.CurrentStatus != BattlerStatusState.Blocking) {
                Battler.ApplyStatusState(BattlerStatusState.Staggered);
            } else {
                sourceBattler.Battler.ApplyStatusState(BattlerStatusState.Staggered);
                if (Battler.CurrentContinuousState == BattlerContinuousState.AutoCounter) {
                    Debug.Log($"{BattlerMono.GetType().Name} \"{BattlerMono.transform.gameObject.name}\" blocked an attack in {BattlerContinuousState.AutoCounter}, beginning counterattack.");
                    _blockSkill.Effect.OnEffectInterrupt(BattlerMono);
                    _currentSkill = _attackSkill;
                }
            }

            StartCoroutine(CreateInvulnerabilityCoroutine(_invulnerabilityDurationSeconds));
        }

        private IEnumerator CreateInvulnerabilityCoroutine(float duration) {
            _isInvulnerable = true;
            yield return new WaitForSeconds(duration);
            _isInvulnerable = false;
        }
    }
}
