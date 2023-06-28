using AWE.Synzza.UnityLayer.Monocomponents;
using System.Collections;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    [CreateAssetMenu(fileName = "NewBlock", menuName = "Scrib/SkillEffect/Block")]
    public class BlockSkillEffectScrib : SkillEffectScrib {
        public override bool IsIndefinite => true;

        public override bool IsEffectActivatable(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler) {
            float doubleRange = 1.5f * sourceBattler.Battler.DefaultMeleeAttackRange;
            return sourceBattler.Battler.CurrentStatus == BattlerStatusState.OK
                && (sourceBattler.transform.position - targetBattler.transform.position).sqrMagnitude < (doubleRange * doubleRange);
        }

        protected override IEnumerator CreateEffectCoroutine(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler, SkillEffectCoroutine.RunningState state) {
            if (sourceBattler.Battler.ApplyStatusState(BattlerStatusState.Blocking)) {
                float doubleRange = 1.5f * sourceBattler.Battler.DefaultMeleeAttackRange;
                float doubleRangeSquared = doubleRange * doubleRange;
                yield return new WaitWhile(() => (sourceBattler.transform.position - targetBattler.transform.position).sqrMagnitude < doubleRangeSquared);
            }

            yield break;
        }

        protected override void EndEffect(IBattlerMonocomponent sourceBattler, IBattlerMonocomponent targetBattler, SkillEffectCoroutine.RunningState state) {
            sourceBattler.Battler.RemoveStatusState(BattlerStatusState.Blocking);
        }
    }
}
