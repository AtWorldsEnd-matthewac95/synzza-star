using AWE.Synzza.UnityLayer.Monocomponents;
using System.Collections;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    [CreateAssetMenu(fileName = "NewHitboxSkill", menuName = "Scrib/SkillEffect/Hitbox")]
    public class HitboxSkillEffectScrib : SkillEffectScrib {
        [SerializeField] protected GameObject _hitbox;
        [SerializeField] protected SpawnProfileScrib _hitboxSpawnProfile;
        [SerializeField] protected SkillEffectDurationProfileScrib _effectDurationProfile;
        [SerializeField] protected SkillAttackRangeProfileScrib _attackRangeProfile;

        public override bool IsIndefinite => _effectDurationProfile != null && _effectDurationProfile.IsIndefinite;

        public override bool IsEffectActivatable(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler) {
            if (sourceBattler.Battler.CurrentStatus != BattlerStatusState.OK) {
                return false;
            }

            if (_attackRangeProfile == null || _attackRangeProfile.IsUsingDefaultAttackRange) {
                float range = sourceBattler.Battler.DefaultMeleeAttackRange;
                return (sourceBattler.transform.position - targetBattler.transform.position).sqrMagnitude < (range * range);
            } else {
                return _attackRangeProfile.AttackRange.IsWithin((sourceBattler.transform.position - targetBattler.transform.position).magnitude);
            }
        }

        protected override IEnumerator CreateEffectCoroutine(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler, SkillEffectCoroutine.RunningState state) {
            if (_effectDurationProfile == null || !sourceBattler.Battler.ApplyStatusState(BattlerStatusState.SkillEffect)) {
                yield break;
            }

            var sourceTransform = sourceBattler.transform;
            var spawnPosition = sourceTransform.position;
            var spawnRotation = Quaternion.identity;

            if (_hitboxSpawnProfile != null) {
                spawnPosition = _hitboxSpawnProfile.FindSpawnPosition(sourceTransform, sourceTransform.lossyScale, sourceTransform.lossyScale);

                if (_hitboxSpawnProfile.IsInheritingSpawnerRotation) {
                    if (_hitboxSpawnProfile.IsIgnoringAnyInheritedRotation) {
                        var spawnEulerAngles = sourceTransform.eulerAngles;
                        var eulerX = _hitboxSpawnProfile.IsIgnoringInheritedXRotation ? 0f : spawnEulerAngles.x;
                        var eulerY = _hitboxSpawnProfile.IsIgnoringInheritedYRotation ? 0f : spawnEulerAngles.y;
                        var eulerZ = _hitboxSpawnProfile.IsIgnoringInheritedZRotation ? 0f : spawnEulerAngles.z;
                        Quaternion.Euler(eulerX, eulerY, eulerZ);
                    } else {
                        spawnRotation = sourceTransform.rotation;
                    }
                }
            }

            state.Hitbox = Instantiate(_hitbox, spawnPosition, spawnRotation);

            if (state.Hitbox.TryGetComponent(out SkillHitboxMonocomponent hitboxMono)) {
                if (hitboxMono.IsInitialized) {
                    if (hitboxMono.SourceBattler != sourceBattler) {
                        Debug.LogError($"{hitboxMono.GetType().Name} {hitboxMono.gameObject.name} appears to be initialized to the wrong value!");
                    }
                } else {
                    hitboxMono.Initialize(sourceBattler);
                }
            } else {
                Debug.LogWarning($"Skill \"{name}\" attempted to make a hitbox without a {typeof(SkillHitboxMonocomponent).Name} component.");
            }

            yield return _effectDurationProfile.IsIndefinite
                ? new WaitWhile(() => IsIndefinite)
                : new WaitForSeconds(_effectDurationProfile.DurationSeconds);

            EndEffect(sourceBattler, targetBattler, state);
        }

        protected override void EndEffect(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler, SkillEffectCoroutine.RunningState state) {
            if (state != null) {
                Destroy(state.Hitbox);
                state.Hitbox = null;
            } else {
                Debug.LogWarning("Hitbox skill was ended without being able to delete its spawned object!");
            }
            sourceBattler.Battler.ApplyStatusState(BattlerStatusState.SkillWindDown);
        }
    }
}
