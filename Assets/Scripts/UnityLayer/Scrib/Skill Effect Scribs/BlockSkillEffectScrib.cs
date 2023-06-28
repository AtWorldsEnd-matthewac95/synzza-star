using AWE.Synzza.UnityLayer.Monocomponents;
using System.Collections;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    [CreateAssetMenu(fileName = "NewBlock", menuName = "Scrib/SkillEffect/Block")]
    public class BlockSkillEffectScrib : SkillEffectScrib {
        public override bool IsIndefinite => true;

        public override bool IsEffectActivatable(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler) {
            float range = sourceBattler.Battler.DefaultMeleeAttackRange;
            return sourceBattler.Battler.CurrentStatus == BattlerStatusState.OK
                && (sourceBattler.transform.position - targetBattler.transform.position).sqrMagnitude < (range * range);
        }

        protected override IEnumerator CreateEffectCoroutine(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler, SkillEffectCoroutine.RunningState state) {
            if (sourceBattler.Battler.ApplyStatusState(BattlerStatusState.Blocking)) {
                float range = sourceBattler.Battler.DefaultMeleeAttackRange;
                range *= range;
                yield return new WaitUntil(() => (sourceBattler.transform.position - targetBattler.transform.position).sqrMagnitude > range);
                EndEffect(sourceBattler, targetBattler, state);
            }

            yield break;
        }

        protected override void EndEffect(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler, SkillEffectCoroutine.RunningState state) {
            sourceBattler.Battler.RemoveStatusState(BattlerStatusState.Blocking);
        }
    }
}
