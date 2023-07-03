using System.Collections.Generic;

namespace AWE.Synzza {
    public abstract class SkillEffect {
        public bool IsInterruptible { get; }
        public abstract bool IsIndefinite { get; }

        public SkillEffect(bool isInterruptible) {
            IsInterruptible = isInterruptible;
        }

        public abstract bool IsEffectActivatible(IBattlerSceneObject source, IBattlerSceneObject target);
        protected abstract IEnumerable<ICoWait> CreateEffectCoroutine(IBattlerSceneObject source, IBattlerSceneObject target, SkillEffectUsageCapture capture);
        protected abstract SkillEffectUsageCapture CreateEffectUsageCapture(in DurationProfile<float> windDownSeconds);
        protected abstract void EndEffect(IBattlerSceneObject source, IBattlerSceneObject target, SkillEffectUsageCapture capture);

        public SkillEffectUsage CreateUsage(IBattlerSceneObject source, IBattlerSceneObject target, in DurationProfile<float> windDownSeconds = default) {
            var capture = CreateEffectUsageCapture(windDownSeconds);
            return new(IsInterruptible || IsIndefinite, CreateEffectCoroutine(source, target, capture).GetEnumerator(), capture);
        }
        public void InterruptEffect(IBattlerSceneObject source, SkillEffectUsageCapture capture) => EndEffect(source, null, capture);
        public void InterruptEffect(IBattlerSceneObject source, IBattlerSceneObject target, SkillEffectUsageCapture capture) => EndEffect(source, target, capture);
    }

    public abstract class SkillEffectUsageCapture {}

    public sealed class SkillEffectUsage {
        public bool IsInterruptible { get; }
        public SkillEffectUsageCapture Capture { get; }

        public IEnumerator<ICoWait> Coroutine { get; private set; }
        public bool IsStale => Coroutine == null;

        public SkillEffectUsage(bool isInterruptible, in IEnumerator<ICoWait> coroutine, SkillEffectUsageCapture capture) {
            IsInterruptible = isInterruptible;
            Coroutine = coroutine;
            Capture = capture;
        }

        public void Cancel() {
            Coroutine = null;
        }
    }
}
