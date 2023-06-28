using AWE.Synzza.UnityLayer.Monocomponents;
using System.Collections;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    public class SkillEffectCoroutine {
        public class RunningState {
            public GameObject Hitbox { get; set; }
            public float WindDownDuration { get; set; }
        }

        public const float RELATIVE_EPSILON = 0.01f;

        public bool IsInterruptible { get; }
        public IEnumerator Coroutine { get; }
        public RunningState State { get; }

        public SkillEffectCoroutine(bool isInterruptible, IEnumerator coroutine, RunningState state) {
            IsInterruptible = isInterruptible;
            Coroutine = coroutine;
            State = state;
        }
    }

    public abstract class SkillEffectScrib : ScriptableObject {
        [SerializeField] private bool _isInterruptible;

        public bool IsInterruptible => _isInterruptible;

        public abstract bool IsIndefinite { get; }
        public abstract bool IsEffectActivatable(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler);
        protected abstract IEnumerator CreateEffectCoroutine(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler, SkillEffectCoroutine.RunningState state);
        protected abstract void EndEffect(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler, SkillEffectCoroutine.RunningState state);
        public void OnEffectInterrupt(IBattlerMonocomponent sourceBattler, SkillEffectCoroutine.RunningState state = null)
            => EndEffect(sourceBattler, null, state);
        public void OnEffectInterrupt(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler, SkillEffectCoroutine.RunningState state)
            => EndEffect(sourceBattler, targetBattler, state);

        public SkillEffectCoroutine ActivateEffect(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler, float windDownDuration = 0f) {
            if (IsEffectActivatable(sourceBattler, targetBattler)) {
                var state = new SkillEffectCoroutine.RunningState {
                    WindDownDuration = windDownDuration
                };
                return new(_isInterruptible || IsIndefinite, CreateEffectCoroutine(sourceBattler, targetBattler, state), state);
            }

            return new(true, null, null);
        }
    }
}
