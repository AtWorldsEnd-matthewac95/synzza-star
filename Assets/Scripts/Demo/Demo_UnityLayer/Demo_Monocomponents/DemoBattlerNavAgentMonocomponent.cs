using AWE.Synzza.UnityLayer;
using System.Collections;
using UnityEngine;

namespace AWE.Synzza.Demo.UnityLayer {
    public abstract class DemoBattlerNavAgentMonocomponent : NavmeshAgentMonocomponent {
        [SerializeField] protected SkillScrib _blockSkill;
        [SerializeField] protected SkillScrib _attackSkill;
        [SerializeField] protected float _invulnerabilityDurationSeconds;

        protected SkillScrib _currentSkill = null;
        protected SkillEffectScribCoroutine _runningSkill = null;
        private bool _isInvulnerable = false;

        public abstract Battler Battler { get; }
        public abstract BattlerMonocomponent BattlerMono { get; }

        private BattlerMonocomponent _targetBattler;
        protected BattlerMonocomponent TargetBattler {
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

            Battler.OnSkillEffectCancelled += OnBattlerCancelEffects;
            Battler.OnUpdateMeleeRules += OnBattlerUpdateMeleeRules;
            Battler.Status.OnSkillWindDown += OnBattlerWindDown;
            Battler.Status.OnBlockStatusChanged += OnBattlerBlockingStatusChanged;
            Battler.Status.OnStationaryStatusChanged += OnBattlerStationaryStatusChanged;
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

        private void OnBattlerCancelEffects(BattlerStatusState cancellingStatus) {
            _currentSkill.Effect.OnEffectInterrupt(BattlerMono, _runningSkill?.State);

            if (_runningSkill != null) {
                StopCoroutine(_runningSkill.Coroutine);
            }

            _runningSkill = null;

            if (Battler.CurrentMeleeRules == BattlerMeleeRules.AutoCounter) {
                _currentSkill = _blockSkill;
            }

            if (cancellingStatus == BattlerStatusState.Staggered) {
                StopAllCoroutines();
                StartCoroutine(CreateStaggerCoroutine(Battler.StaggerProfile.DurationSeconds));
            }
        }

        protected virtual void OnBattlerWindDown() {
            StartCoroutine(CreateWindDownCoroutine(_runningSkill.State.WindDownDuration));
        }

        private void OnBattlerUpdateMeleeRules(bool isSameState, BattlerMeleeRules newState) {
            if (isSameState) {
                return;
            }

            if (_runningSkill?.IsInterruptible ?? false) {
                _currentSkill.Effect.OnEffectInterrupt(BattlerMono, _runningSkill.State);
            }

            switch (newState) {

                case BattlerMeleeRules.AutoBlock:
                case BattlerMeleeRules.AutoCounter:
                    _currentSkill = _blockSkill;
                    break;

                case BattlerMeleeRules.AutoAttack:
                case BattlerMeleeRules.OpportuneAttack:
                    _currentSkill = _attackSkill;
                    break;

            }
        }

        protected abstract void UpdateContinuousState();

        private IEnumerator CreateSkillCoroutine() {
            _runningSkill = _currentSkill.Effect.CreateEffectCoroutine(BattlerMono, _targetBattler, _currentSkill.WindDownDurationSeconds);
            var status = Battler.Status;
            status.ApplyState(BattlerStatusState.SkillWindUp);

            if (_runningSkill.Coroutine != null) {
                Debug.Log($"{GetType().Name} \"{gameObject.name}\" created the skill successfully.");

                yield return new WaitForSeconds(_currentSkill.WindUpDurationSeconds);
                status.ApplyState(BattlerStatusState.SkillEffect);
                StartCoroutine(_runningSkill.Coroutine);
            } else {
                Debug.Log($"{GetType().Name} \"{gameObject.name}\" failed to create the skill.");

                status.RemoveState(BattlerStatusState.SkillWindUp);
                yield break;
            }
        }

        private IEnumerator CreateWindDownCoroutine(float windDownDurationSeconds) {
            if (Battler.CurrentMeleeRules == BattlerMeleeRules.AutoCounter) {
                _currentSkill = _blockSkill;
            }
            yield return new WaitForSeconds(windDownDurationSeconds);
            Battler.Status.RemoveState(BattlerStatusState.SkillWindDown);
            _runningSkill = null;
        }

        private IEnumerator CreateStaggerCoroutine(float staggerDurationSeconds) {
            yield return new WaitForSeconds(staggerDurationSeconds);
            Battler.Status.RemoveState(BattlerStatusState.Staggered);
        }

        protected override void Update() {
            base.Update();

            if (_targetBattler == null) {
                return;
            }

            bool isOpportune = Battler.CurrentMeleeRules != BattlerMeleeRules.OpportuneAttack || _targetBattler.Battler.Status.IsVulnerable;

            if (isOpportune && _currentSkill.Effect.IsEffectActivatable(BattlerMono, _targetBattler)) {
                StartCoroutine(CreateSkillCoroutine());
            }
        }

        protected virtual void OnCollisionEnter(Collision collision) {
            ReactToAttackHitbox(collision.collider);
        }

        protected virtual void OnTriggerEnter(Collider other) {
            ReactToAttackHitbox(other);
        }

        protected virtual void ReactToAttackHitbox(Collider collider) {
            if (_isInvulnerable || !collider.CompareTag("Attacks") || !collider.gameObject.TryGetComponent(out SkillHitboxColliderMonocomponent colliderMono) || !colliderMono.IsInitialized) {
                return;
            }

            var sourceBattler = colliderMono.Parent.SourceBattler;

            if (sourceBattler.Battler.FactionID == Battler.FactionID) {
                return;
            }

            if (sourceBattler.transform.gameObject.TryGetComponent(out DemoBattlerNavAgentMonocomponent otherNavAgent)) {
                otherNavAgent.PickNewDestination();
            }
            PickNewDestination();

            if (Battler.Status.Current != BattlerStatusState.Blocking) {
                Battler.Status.ApplyState(BattlerStatusState.Staggered);
            } else {
                sourceBattler.Battler.Status.ApplyState(BattlerStatusState.Staggered);
                if (Battler.CurrentMeleeRules == BattlerMeleeRules.AutoCounter) {
                    Debug.Log($"{BattlerMono.GetType().Name} \"{BattlerMono.transform.gameObject.name}\" blocked an attack in {BattlerMeleeRules.AutoCounter}, beginning counterattack.");
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
