using System.Collections.Generic;

namespace AWE.Synzza {
    public class BlockSkillEffect : SkillEffect {
        protected class Capture : SkillEffectUsageCapture {}

        public BlockSkillEffect(SynzzaGame game) : base(game, isInterruptible: true) {}

        public override bool IsIndefinite => true;
        protected override SkillEffectUsageCapture CreateEffectUsageCapture(Skill _, IBattlerWorldObject __, in DurationProfile<float> ___) => new Capture();

        public override bool IsEffectActivatible(IBattlerWorldObject source, IBattlerWorldObject target) {
            float range = source.Battler.InnateMeleeAttackRange;
            return source.Battler.Status.Current == BattlerStatusState.OK
                && (source.WorldPosition - target.WorldPosition).MagnitudeSquared < range * range;
        }

        protected override IEnumerable<ICoWait> CreateEffectCoroutine(IBattlerWorldObject source, IBattlerWorldObject target, SkillEffectUsageCapture capture) {
            if (source.Battler.Status.ApplyState(BattlerStatusState.Blocking)) {
                float range = source.Battler.InnateMeleeAttackRange;
                range *= range;

                yield return new CoWaitUntil(() => (source.WorldPosition - target.WorldPosition).MagnitudeSquared > range);

                EndEffect(source, target, capture);
            }

            yield break;
        }

        protected override void EndEffect(IBattlerWorldObject source, IBattlerWorldObject target, SkillEffectUsageCapture capture) {
            source.Battler.Status.RemoveState(BattlerStatusState.Blocking);
        }
    }
}
