using System.Collections.Generic;

namespace AWE.Synzza {
    public abstract class SkillEffect : SynzzaGameDependent {
        public bool IsInterruptible { get; }
        public abstract bool IsIndefinite { get; }

        public SkillEffect(SynzzaGame game, bool isInterruptible) : base(game) {
            IsInterruptible = isInterruptible;
        }

        public abstract bool IsEffectActivatible(IBattlerWorldObject source, IBattlerWorldObject target);
        protected abstract IEnumerable<ICoWait> CreateEffectCoroutine(IBattlerWorldObject source, IBattlerWorldObject target, SkillEffectUsageCapture capture);
        protected abstract SkillEffectUsageCapture CreateEffectUsageCapture(Skill sourceSkill, IBattlerWorldObject sourceBattler, in DurationProfile<float> windDownSeconds);
        protected abstract void EndEffect(IBattlerWorldObject source, IBattlerWorldObject target, SkillEffectUsageCapture capture);

        public SkillEffectUsage CreateUsage(Skill sourceSkill, IBattlerWorldObject sourceBattler, IBattlerWorldObject target, in DurationProfile<float> windDownSeconds = default) {
            var capture = CreateEffectUsageCapture(sourceSkill, sourceBattler, windDownSeconds);
            return new(this, IsInterruptible || IsIndefinite, CreateEffectCoroutine(sourceBattler, target, capture).GetEnumerator(), capture);
        }
        public void InterruptEffect(IBattlerWorldObject source, SkillEffectUsageCapture capture) => EndEffect(source, null, capture);
        public void InterruptEffect(IBattlerWorldObject source, IBattlerWorldObject target, SkillEffectUsageCapture capture) => EndEffect(source, target, capture);
    }

    public abstract class SkillEffectUsageCapture {}

    public sealed class SkillEffectUsage {
        public SkillEffect Effect;
        public bool IsInterruptible { get; }
        public SkillEffectUsageCapture Capture { get; }

        public IEnumerator<ICoWait> Coroutine { get; private set; }
        public bool IsStale => Coroutine == null;

        public SkillEffectUsage(SkillEffect effect, bool isInterruptible, in IEnumerator<ICoWait> coroutine, SkillEffectUsageCapture capture) {
            Effect = effect;
            IsInterruptible = isInterruptible;
            Coroutine = coroutine;
            Capture = capture;
        }
    }
}
