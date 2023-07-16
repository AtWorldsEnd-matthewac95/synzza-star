using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [CreateAssetMenu(fileName = "NewBlock", menuName = "Scrib/SkillEffect/Block")]
    public class BlockSkillEffectScrib : SkillEffectScrib {
        public BlockSkillEffect ToBlockSkillEffect() => new(SingletonSynzzaGame.Current);
        public override SkillEffect ToSkillEffect() => ToBlockSkillEffect();
    }
}
