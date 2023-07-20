using System;
using System.Collections.Generic;

namespace AWE.Synzza {
    public class HitboxSkillEffect : SkillEffect {
        public class Capture : SkillEffectUsageCapture {
            public ISkillHitboxWorldObjectTemplate HitboxTemplate { get; }
            public ISkillHitboxWorldObject Hitbox { get; set; }
            public DurationProfile<float> WindDownSeconds { get; }
            public bool IsIndefinite { get; set; }

            public Capture(in ISkillHitboxWorldObjectTemplate hitboxTemplate, in DurationProfile<float> windDownSeconds, bool isIndefinite = false) {
                HitboxTemplate = hitboxTemplate.Clone();
                Hitbox = null;
                WindDownSeconds = windDownSeconds;
                IsIndefinite = isIndefinite;
            }
        }

        private readonly ISkillHitboxWorldObjectTemplate _template;

        public ISkillHitboxWorldObjectTemplate HitboxTemplate => _template.Clone();
        public float HitboxLifeDurationSeconds { get; }
        public override bool IsIndefinite { get; }
        public byte AttackRangeProfileID { get; }
        public byte SpawnLocationProfileID { get; }

        public virtual bool IsFriendlyFire => true;
        public virtual bool IsBattlerStationaryDuringEffect => true;

        private HitboxSkillEffect(
            SynzzaGame game,
            in ISkillHitboxWorldObjectTemplate hitboxTemplate,
            bool isInterruptible,
            bool isIndefinite,
            float hitboxLifeDurationSeconds,
            byte spawnLocationProfileID,
            byte attackRangeProfileID
        ) : base(game, isInterruptible) {
            _template = hitboxTemplate.Clone();
            IsIndefinite = isIndefinite;
            HitboxLifeDurationSeconds = hitboxLifeDurationSeconds;
            SpawnLocationProfileID = spawnLocationProfileID;
            AttackRangeProfileID = attackRangeProfileID;
        }
        public HitboxSkillEffect(
            SynzzaGame game,
            in ISkillHitboxWorldObjectTemplate hitboxTemplate,
            bool isInterruptible,
            (bool isIndefinite, float hitboxLifeDurationSeconds) tuple,
            byte spawnLocationProfileID = SpawnLocationProfile.INVALID_ID,
            byte attackRangeProfileID = SkillAttackRangeProfile.INVALID_ID
        ) : this(game, hitboxTemplate, isInterruptible, tuple.isIndefinite, tuple.hitboxLifeDurationSeconds, spawnLocationProfileID, attackRangeProfileID) {}
        public HitboxSkillEffect(
            SynzzaGame game,
            in ISkillHitboxWorldObjectTemplate hitboxTemplate,
            bool isInterruptible,
            float hitboxLifeDurationSeconds,
            byte spawnLocationProfileID = SpawnLocationProfile.INVALID_ID,
            byte attackRangeProfileID = SkillAttackRangeProfile.INVALID_ID
        ) : this(game, hitboxTemplate, isInterruptible, false, hitboxLifeDurationSeconds, spawnLocationProfileID, attackRangeProfileID) {}
        public HitboxSkillEffect(
            SynzzaGame game,
            in ISkillHitboxWorldObjectTemplate hitboxTemplate,
            bool isInterruptible,
            byte spawnLocationProfileID = SpawnLocationProfile.INVALID_ID,
            byte attackRangeProfileID = SkillAttackRangeProfile.INVALID_ID
        ) : this(game, hitboxTemplate, isInterruptible, true, default, spawnLocationProfileID, attackRangeProfileID) {}

        public override bool IsEffectActivatible(IBattlerWorldObject source, IBattlerWorldObject target) {
            if (source.Battler.Status.Current != BattlerStatusState.OK) {
                return false;
            }

            var mag2 = (source.WorldPosition - target.WorldPosition).MagnitudeSquared;

            if (AttackRangeProfileID == SkillAttackRangeProfile.INVALID_ID) {
                var range = source.Battler.InnateMeleeAttackRange;
                return mag2 < (range * range);
            } else {
                return _game.SkillAttackRanges[AttackRangeProfileID].Range.IsWithin((float)Math.Sqrt(mag2));
            }
        }

        protected override SkillEffectUsageCapture CreateEffectUsageCapture(Skill sourceSkill, IBattlerWorldObject sourceBattler, in DurationProfile<float> windDownSeconds) {
            _template.SetSourceBattler(sourceBattler);
            _template.SetSkillHitbox(new(sourceBattler, sourceSkill, IsFriendlyFire));
            var capture = new Capture(_template, windDownSeconds, IsIndefinite);
            _template.ClearSkillHitbox();
            _template.ClearSourceBattler();
            return capture;
        }

        protected override IEnumerable<ICoWait> CreateEffectCoroutine(IBattlerWorldObject source, IBattlerWorldObject target, SkillEffectUsageCapture c) {
            if (!source.Battler.Status.ApplyState(BattlerStatusState.SkillEffect) || c is not Capture capture) {
                yield break;
            }

            var world = _game.GetCurrentWorld();
            capture.Hitbox = (
                SpawnLocationProfileID == SpawnLocationProfile.INVALID_ID
                ? world.SpawnObjectFromTemplate(capture.HitboxTemplate, source.WorldPosition, source.WorldRotation)
                : world.SpawnObjectFromTemplate(capture.HitboxTemplate, source, _game.SpawnLocations[SpawnLocationProfileID])
            ) as ISkillHitboxWorldObject;

            yield return capture.IsIndefinite ? new CoWaitWhile(() => capture.IsIndefinite) : new CoWaitForSeconds(capture.WindDownSeconds.Duration);

            EndEffect(source, target, capture);
        }

        protected override void EndEffect(IBattlerWorldObject source, IBattlerWorldObject target, SkillEffectUsageCapture c) {
            if (c is Capture capture && capture.Hitbox != null) {
                _game.GetCurrentWorld().DestroyObject(capture.Hitbox);
                capture.Hitbox = null;
            }
            source.Battler.Status.ApplyState(BattlerStatusState.SkillWindDown);
        }
    }
}
