using System.Collections;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [CreateAssetMenu(fileName = "NewBlock", menuName = "Scrib/SkillEffect/Block")]
    public class BlockSkillEffectScrib : SkillEffectScrib {
        public override bool IsIndefinite => true;

        public override bool IsEffectActivatable(BattlerMonocomponent sourceBattler, BattlerMonocomponent targetBattler) {
            float range = sourceBattler.Battler.InnateMeleeAttackRange;
            return sourceBattler.Battler.Status.Current == BattlerStatusState.OK
                && (sourceBattler.transform.position - targetBattler.transform.position).sqrMagnitude < (range * range);
        }

        protected override IEnumerator CreateEffectCoroutine(BattlerMonocomponent sourceBattler, BattlerMonocomponent targetBattler, SkillEffectScribCoroutine.RunningState state) {
            if (sourceBattler.Battler.Status.ApplyState(BattlerStatusState.Blocking)) {
                float range = sourceBattler.Battler.InnateMeleeAttackRange;
                range *= range;
                yield return new WaitUntil(() => (sourceBattler.transform.position - targetBattler.transform.position).sqrMagnitude > range);
                EndEffect(sourceBattler, targetBattler, state);
            }

            yield break;
        }

        protected override void EndEffect(BattlerMonocomponent sourceBattler, BattlerMonocomponent targetBattler, SkillEffectScribCoroutine.RunningState state) {
            sourceBattler.Battler.Status.RemoveState(BattlerStatusState.Blocking);
        }
    }
}
