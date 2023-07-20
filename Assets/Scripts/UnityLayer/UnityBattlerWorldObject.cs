using System.Collections.Generic;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public class UnityBattlerWorldObject : UnityWorldObject, IBattlerWorldObject {
        public const float INVINCIBLE_DURATION = 0.3f;

        public Battler Battler { get; }
        public SkillUsage CurrentSkillUsage { get; private set; }
        public bool IsInvincible { get; protected set; }

        public UnityBattlerWorldObject(Battler battler, Transform T, bool isMobile = true) : base(T, isMobile) {
            Battler = battler;
            SubscribeToBattlerEvents(Battler);
            CurrentSkillUsage = null;
        }
        public UnityBattlerWorldObject(Battler battler, MonoBehaviour mono, bool isMobile = true) : base(mono, isMobile) {
            Battler = battler;
            SubscribeToBattlerEvents(Battler);
            CurrentSkillUsage = null;
        }

        private void SubscribeToBattlerEvents(Battler battler) {
            battler.OnSkillEffectCancelled += OnBattlerSkillEffectCancelled;
            battler.Status.OnSkillWindDown += OnBattlerSkillWindDown;
            battler.OnUpdateMeleeRules += OnBattlerUpdateMeleeRules;
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
                sourceBattler.Status.ApplyState(BattlerStatusState.Staggered);

                if (Battler.CurrentMeleeRules == BattlerMeleeRules.AutoCounter) {
                    if (Battler.CurrentBlockTargetBattler == null) {
                        Battler.SetBlockTargetBattler(sourceBattlerObject);
                    }

                    StartCoroutine(Battler.CurrentCounterSkill.CreateCoroutine(this, Battler.CurrentBlockTargetBattler));
                }
            }

            StartCoroutine(CreateInvincibilityCoroutine());
        }

        protected IEnumerable<ICoWait> CreateInvincibilityCoroutine(float duration = INVINCIBLE_DURATION) {
            IsInvincible = true;
            yield return new CoWaitForSeconds(duration);
            IsInvincible = false;
        }

        protected void EndCurrentSkillUsage() {
            CurrentSkillUsage.Skill.Effect.InterruptEffect(this, CurrentSkillUsage.EffectUsage.Capture);
            StopCoroutine(CurrentSkillUsage.EffectUsage.Coroutine);
            CurrentSkillUsage = null;
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

        protected IEnumerator<ICoWait> CreateStaggerCoroutine(float staggerDurationSeconds) {
            yield return new CoWaitForSeconds(staggerDurationSeconds);
            Battler.Status.RemoveState(BattlerStatusState.Staggered);
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

        protected virtual void OnBattlerSkillWindDown() {
            if (CurrentSkillUsage.EffectUsage.Effect.IsIndefinite) {
                OnBattlerSkillEffectCancelled(BattlerStatusState.SkillWindDown);
            } else if (CurrentSkillUsage.EffectUsage.Capture is HitboxSkillEffect.Capture capture) {
                StartCoroutine(CreateWindDownCoroutine(capture.WindDownSeconds.Duration));
            }
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
