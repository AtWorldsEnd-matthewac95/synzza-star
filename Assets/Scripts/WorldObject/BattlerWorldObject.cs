using System.Collections.Generic;

namespace AWE.Synzza {
    // TODO - Replace this interface with an abstract base class.
    public interface IBattlerWorldObject : IMutableWorldObject {
        Battler Battler { get; }
        SkillUsage CurrentSkillUsage { get; }
        bool IsInvincible { get; }

        event PlayerBattlerNeedsInputDelegate OnPlayerBattlerNeedsInput;

        bool TrySetCurrentSkillUsage(SkillUsage usage);
        void ReactToSkillHitbox(in ISkillHitboxWorldObject hitbox);

        void CheckIfPlayerBattlerNeedsInput(BattlerStatusState oldStatusState, BattlerStatusState newStatusState);
        void CheckIfPlayerBattlerNeedsInput();
    }

    public class BattlerWorldObject : WorldObject, IBattlerWorldObject {
        public const float INVINCIBLE_DURATION = 0.3f;

        public Battler Battler { get; }
        public SkillUsage CurrentSkillUsage { get; protected set; }
        public bool IsInvincible { get; protected set; }

        public event PlayerBattlerNeedsInputDelegate OnPlayerBattlerNeedsInput;

        public BattlerWorldObject(Impl impl, Battler battler) : base(impl) {
            Battler = battler;
            CurrentSkillUsage = null;
            IsInvincible = false;

            Battler.OnSkillEffectCancelled += OnBattlerSkillEffectCancelled;
            Battler.Status.OnSkillWindDown += OnBattlerSkillWindDown;
            Battler.OnUpdateMeleeRules += OnBattlerUpdateMeleeRules;
            Battler.Status.OnStateRemoved += CheckIfPlayerBattlerNeedsInput;
        }

        public bool TrySetCurrentSkillUsage(SkillUsage usage) {
            if (!CurrentSkillUsage?.IsStale ?? false) {
                if (CurrentSkillUsage.EffectUsage.IsInterruptible) {
                    EndCurrentSkillUsage();
                } else {
                    return false;
                }
            }

            CurrentSkillUsage = usage;
            return true;
        }

        protected void EndCurrentSkillUsage() {
            CurrentSkillUsage.Skill.Effect.InterruptEffect(this, CurrentSkillUsage.EffectUsage.Capture);
            StopCoroutine(CurrentSkillUsage.EffectUsage.Coroutine);
            CurrentSkillUsage = null;
        }

        public void ReactToSkillHitbox(in ISkillHitboxWorldObject hitbox) {
            var sourceBattlerObject = hitbox.Hitbox.SourceBattler;
            var sourceBattler = sourceBattlerObject.Battler;

            if (sourceBattlerObject == this || (!hitbox.Hitbox.IsFriendlyFire && Battler.FactionID == sourceBattler.FactionID)) {
                return;
            }

            if (Battler.Status.Current != BattlerStatusState.Blocking) {
                Battler.Status.ApplyState(BattlerStatusState.Staggered);
                StartCoroutine(CreateStaggerCoroutine(Battler.StaggerProfile.DurationSeconds));
            } else {
                if (Battler.CurrentMeleeRules == BattlerMeleeRules.AutoCounter) {
                    if (Battler.CurrentBlockTargetBattler == null) {
                        Battler.SetBlockTargetBattler(sourceBattlerObject);
                    }

                    StartCoroutine(Battler.CurrentCounterSkill.CreateCoroutine(this, Battler.CurrentBlockTargetBattler));
                }
            }

            StartCoroutine(CreateInvincibilityCoroutine());
        }

        public void CheckIfPlayerBattlerNeedsInput(BattlerStatusState oldStatusState, BattlerStatusState newStatusState) {
            if (newStatusState == BattlerStatusState.OK) {
                _CheckIfPlayerBattlerNeedsInput();
            }
        }

        public void CheckIfPlayerBattlerNeedsInput() {
            if (Battler.Status.Current == BattlerStatusState.OK) {
                _CheckIfPlayerBattlerNeedsInput();
            }
        }

        private void _CheckIfPlayerBattlerNeedsInput() {
            if (Battler.IsPlayerBattler && IsPlayerNeedingInput()) {
                OnPlayerBattlerNeedsInput?.Invoke(this);
            }
        }

        protected virtual bool IsPlayerNeedingInput() => Battler.CurrentTargetBattler == null;

        protected IEnumerable<ICoWait> CreateInvincibilityCoroutine(float duration = INVINCIBLE_DURATION) {
            IsInvincible = true;
            yield return new CoWaitForSeconds(duration);
            IsInvincible = false;
        }

        protected IEnumerator<ICoWait> CreateStaggerCoroutine(float staggerDurationSeconds) {
            yield return new CoWaitForSeconds(staggerDurationSeconds);
            Battler.Status.RemoveState(BattlerStatusState.Staggered);
        }

        protected virtual void OnBattlerSkillEffectCancelled(BattlerStatusState cancellingStatus) {
            if (CurrentSkillUsage == null) {
                return;
            }

            EndCurrentSkillUsage();

            /*
            if (Battler.CurrentMeleeRules == BattlerMeleeRules.AutoCounter) {
                _currentSkill = SingletonSynzzaGame.Current.Skills[_blockSkill.ID];
            }
            */

            if (cancellingStatus == BattlerStatusState.Staggered) {
                StopAllCoroutines();
                StartCoroutine(CreateStaggerCoroutine(Battler.StaggerProfile.DurationSeconds));
            }
        }

        protected virtual void OnBattlerSkillWindDown() {
            if (CurrentSkillUsage.EffectUsage.Effect.IsIndefinite) {
                OnBattlerSkillEffectCancelled(BattlerStatusState.SkillWindDown);
            } else if (CurrentSkillUsage.EffectUsage.Capture is HitboxSkillEffect.Capture capture) {
                StartCoroutine(CreateWindDownCoroutine(capture.WindDownSeconds.Duration));
            }
        }

        protected IEnumerator<ICoWait> CreateWindDownCoroutine(float windDownDurationSeconds) {
            /*
            if (Battler.CurrentMeleeRules == BattlerMeleeRules.AutoCounter) {
                _currentSkill = SingletonSynzzaGame.Current.Skills[_blockSkill.ID];
            }
            */
            yield return new CoWaitForSeconds(windDownDurationSeconds);
            Battler.Status.RemoveState(BattlerStatusState.SkillWindDown);
            CurrentSkillUsage = null;
        }

        protected virtual void OnBattlerUpdateMeleeRules(bool isSameState, BattlerMeleeRules newState) {
            if (isSameState) {
                return;
            }

            if (CurrentSkillUsage?.EffectUsage?.IsInterruptible ?? false) {
                EndCurrentSkillUsage();
            }
        }
    }
}
