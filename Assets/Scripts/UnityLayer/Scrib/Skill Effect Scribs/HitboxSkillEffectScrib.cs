using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [CreateAssetMenu(fileName = "NewHitboxSkill", menuName = "Scrib/SkillEffect/Hitbox")]
    public class HitboxSkillEffectScrib : SkillEffectScrib {
        [SerializeField] protected GameObject _hitbox;
        [SerializeField] protected SpawnLocationProfileScrib _hitboxSpawnLocationProfile;
        [SerializeField] protected SkillEffectDurationProfileScrib _effectDurationProfile;
        [SerializeField] protected SkillAttackRangeProfileScrib _attackRangeProfile;

        public HitboxSkillEffect ToHitboxSkillEffect() => new(
            SingletonSynzzaGame.Current,
            new UnitySkillHitboxPrefab(_hitbox),
            _isInterruptible,
            _effectDurationProfile.ToTuple(),
            _hitboxSpawnLocationProfile.ID,
            _attackRangeProfile.ID
        );
        public override SkillEffect ToSkillEffect() => ToHitboxSkillEffect();

        /*
        public override bool IsIndefinite => _effectDurationProfile != null && _effectDurationProfile.IsIndefinite;

        public override bool IsEffectActivatable(BattlerMonocomponent sourceBattler, BattlerMonocomponent targetBattler) {
            if (sourceBattler.Battler.Status.Current != BattlerStatusState.OK) {
                return false;
            }

            if (_attackRangeProfile == null || _attackRangeProfile.IsUsingInnateAttackRange) {
                float range = sourceBattler.Battler.InnateMeleeAttackRange;
                return (sourceBattler.transform.position - targetBattler.transform.position).sqrMagnitude < (range * range);
            } else {
                return _attackRangeProfile.AttackRange.IsWithin((sourceBattler.transform.position - targetBattler.transform.position).magnitude);
            }
        }

        protected override IEnumerator CreateEffectCoroutine(BattlerMonocomponent sourceBattler, BattlerMonocomponent targetBattler, SkillEffectScribCoroutine.RunningState state) {
            if (_effectDurationProfile == null || !sourceBattler.Battler.Status.ApplyState(BattlerStatusState.SkillEffect)) {
                yield break;
            }

            var sourceTransform = sourceBattler.transform;
            var spawnPosition = sourceTransform.position;
            var spawnRotation = Quaternion.identity;

            if (_hitboxSpawnProfile != null) {
                var sceneObject = new UnityWorldObject(sourceTransform);
                var spawnProfile = _hitboxSpawnProfile.ToSpawnLocationProfile();
                spawnPosition = spawnProfile.FindSpawnPosition(sceneObject, sourceTransform.lossyScale.ToFloat3(), sourceTransform.lossyScale.ToFloat3()).ToVector3();
                spawnRotation = spawnProfile.FindSpawnRotation(sceneObject).ToQuaternion();
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

        protected override void EndEffect(BattlerMonocomponent sourceBattler, BattlerMonocomponent targetBattler, SkillEffectScribCoroutine.RunningState state) {
            if (state != null) {
                Destroy(state.Hitbox);
                state.Hitbox = null;
            } else {
                Debug.LogWarning("Hitbox skill was ended without being able to delete its spawned object!");
            }
            sourceBattler.Battler.Status.ApplyState(BattlerStatusState.SkillWindDown);
        }
        */
    }
}
