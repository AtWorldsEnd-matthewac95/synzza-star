using AWE.Synzza.UnityLayer;
using System.Collections;
using UnityEngine;

namespace AWE.Synzza.Demo.UnityLayer {
    public abstract class DemoBattlerNavAgentMonocomponent : NavmeshAgentMonocomponent {
        [SerializeField] protected SkillScrib _blockSkill;
        [SerializeField] protected SkillScrib _attackSkill;
        [SerializeField] protected float _invulnerabilityDurationSeconds;

        protected UnityBattlerWorldObject _battlerObject = null;
        protected UnityBattlerWorldObject _targetBattlerObject = null;
        protected Skill _currentSkill = null;
        protected SkillEffectUsage _runningSkill = null;
        protected UnityCoroutine _runningSkillCoroutine = null;
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
                _targetBattlerObject = _targetBattler == null ? null : new UnityBattlerWorldObject(_targetBattler);
            }
        }

        protected override void Awake() {
            _currentSkill = SingletonSynzzaGame.Current.Skills[_attackSkill.ID];
            base.Awake();
        }

        protected override void Start() {
            base.Start();

            Battler.OnSkillEffectCancelled += OnBattlerCancelEffects;
            Battler.OnUpdateMeleeRules += OnBattlerUpdateMeleeRules;
            Battler.Status.OnSkillWindDown += OnBattlerWindDown;
            Battler.Status.OnBlockStatusChanged += OnBattlerBlockingStatusChanged;
            Battler.Status.OnStationaryStatusChanged += OnBattlerStationaryStatusChanged;

            _battlerObject = new UnityBattlerWorldObject(BattlerMono);
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
            _currentSkill.Effect.InterruptEffect(_battlerObject, _runningSkill.Capture);

            if (_runningSkill != null && _runningSkillCoroutine != null) {
                StopCoroutine(_runningSkillCoroutine);
            }

            _runningSkill = null;
            _runningSkillCoroutine = null;

            if (Battler.CurrentMeleeRules == BattlerMeleeRules.AutoCounter) {
                _currentSkill = SingletonSynzzaGame.Current.Skills[_blockSkill.ID];
            }

            if (cancellingStatus == BattlerStatusState.Staggered) {
                StopAllCoroutines();
                StartCoroutine(CreateStaggerCoroutine(Battler.StaggerProfile.DurationSeconds));
            }
        }

        protected virtual void OnBattlerWindDown() {
            if (_runningSkill.Capture is HitboxSkillEffect.Capture hitboxSkillCapture && !hitboxSkillCapture.IsIndefinite) {
                StartCoroutine(CreateWindDownCoroutine(hitboxSkillCapture.WindDownSeconds.Duration));
            }
        }

        private void OnBattlerUpdateMeleeRules(bool isSameState, BattlerMeleeRules newState) {
            if (isSameState) {
                return;
            }

            if (_runningSkill?.IsInterruptible ?? false) {
                _currentSkill.Effect.InterruptEffect(_battlerObject, _runningSkill.Capture);
            }

            switch (newState) {

                case BattlerMeleeRules.AutoBlock:
                case BattlerMeleeRules.AutoCounter:
                    _currentSkill = SingletonSynzzaGame.Current.Skills[_blockSkill.ID];
                    break;

                case BattlerMeleeRules.AutoAttack:
                case BattlerMeleeRules.OpportuneAttack:
                    _currentSkill = SingletonSynzzaGame.Current.Skills[_attackSkill.ID];
                    break;

            }
        }

        protected abstract void UpdateContinuousState();

        private IEnumerator CreateSkillCoroutine() {
            _runningSkill = _currentSkill.Effect.CreateUsage(_currentSkill, _battlerObject, _targetBattlerObject, _currentSkill.WindDownSeconds);
            var status = Battler.Status;
            status.ApplyState(BattlerStatusState.SkillWindUp);

            if (_runningSkill.Coroutine != null) {
                Debug.Log($"{GetType().Name} \"{gameObject.name}\" created the skill successfully.");

                yield return new WaitForSeconds(_currentSkill.WindUpSeconds.Duration);
                status.ApplyState(BattlerStatusState.SkillEffect);
                _runningSkillCoroutine = new UnityCoroutine(_runningSkill.Coroutine);
                StartCoroutine(_runningSkillCoroutine);
            } else {
                Debug.Log($"{GetType().Name} \"{gameObject.name}\" failed to create the skill.");

                status.RemoveState(BattlerStatusState.SkillWindUp);
                yield break;
            }
        }

        private IEnumerator CreateWindDownCoroutine(float windDownDurationSeconds) {
            if (Battler.CurrentMeleeRules == BattlerMeleeRules.AutoCounter) {
                _currentSkill = SingletonSynzzaGame.Current.Skills[_blockSkill.ID];
            }
            yield return new WaitForSeconds(windDownDurationSeconds);
            Battler.Status.RemoveState(BattlerStatusState.SkillWindDown);
            _runningSkill = null;
            _runningSkillCoroutine = null;
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

            if (Battler == null) {
                Debug.LogWarning($"My name is \"{gameObject.name}\", and something weird is happening with me! I don't have a Battler instance!");
            } else if (_targetBattler.Battler == null) {
                Debug.LogWarning($"My name is \"{gameObject.name}\", and something weird is happening with me! My target battler \"{_targetBattler.gameObject.name}\" doesn't have a Battler instance!");
            } else if (_targetBattler.Battler.Status == null) {
                Debug.LogWarning($"My name is \"{gameObject.name}\", and something weird is happening with me! My target battler (name: \"{_targetBattler.gameObject.name}\", battler: \"{_targetBattler.Battler.DisplayName}\") doesn't have a BattlerStatus!");
            } else {
                bool isOpportune = Battler.CurrentMeleeRules != BattlerMeleeRules.OpportuneAttack || _targetBattler.Battler.Status.IsVulnerable;

                if (isOpportune && _currentSkill.Effect.IsEffectActivatible(_battlerObject, _targetBattlerObject)) {
                    StartCoroutine(CreateSkillCoroutine());
                }
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
                    _currentSkill.Effect.InterruptEffect(_battlerObject, _runningSkill.Capture);
                    _currentSkill = SingletonSynzzaGame.Current.Skills[_attackSkill.ID];
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
