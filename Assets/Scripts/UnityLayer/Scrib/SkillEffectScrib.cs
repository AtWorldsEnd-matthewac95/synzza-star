using System.Collections;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public class SkillEffectScribCoroutine {
        public class RunningState {
            public GameObject Hitbox { get; set; }
            public float WindDownDuration { get; set; }
        }

        public const float RELATIVE_EPSILON = 0.01f;

        public bool IsInterruptible { get; }
        public IEnumerator Coroutine { get; }
        public RunningState State { get; }

        public SkillEffectScribCoroutine(bool isInterruptible, IEnumerator coroutine, RunningState state) {
            IsInterruptible = isInterruptible;
            Coroutine = coroutine;
            State = state;
        }
    }

    public abstract class SkillEffectScrib : ScriptableObject {
        [SerializeField] private bool _isInterruptible;

        public bool IsInterruptible => _isInterruptible;

        public abstract bool IsIndefinite { get; }
        public abstract bool IsEffectActivatable(BattlerMonocomponent sourceBattler, BattlerMonocomponent targetBattler);
        protected abstract IEnumerator CreateEffectCoroutine(BattlerMonocomponent sourceBattler, BattlerMonocomponent targetBattler, SkillEffectScribCoroutine.RunningState state);
        protected abstract void EndEffect(BattlerMonocomponent sourceBattler, BattlerMonocomponent targetBattler, SkillEffectScribCoroutine.RunningState state);
        public void OnEffectInterrupt(BattlerMonocomponent sourceBattler, SkillEffectScribCoroutine.RunningState state = null)
            => EndEffect(sourceBattler, null, state);
        public void OnEffectInterrupt(BattlerMonocomponent sourceBattler, BattlerMonocomponent targetBattler, SkillEffectScribCoroutine.RunningState state)
            => EndEffect(sourceBattler, targetBattler, state);

        public SkillEffectScribCoroutine CreateEffectCoroutine(BattlerMonocomponent sourceBattler, BattlerMonocomponent targetBattler, float windDownDuration = 0f) {
            if (IsEffectActivatable(sourceBattler, targetBattler)) {
                var state = new SkillEffectScribCoroutine.RunningState {
                    WindDownDuration = windDownDuration
                };
                return new(_isInterruptible || IsIndefinite, CreateEffectCoroutine(sourceBattler, targetBattler, state), state);
            }

            return new(true, null, null);
        }
    }
}
