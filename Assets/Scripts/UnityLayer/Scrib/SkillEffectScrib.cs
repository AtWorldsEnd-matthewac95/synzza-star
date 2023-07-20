using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public abstract class SkillEffectScrib : ScriptableObject {
        [SerializeField] protected bool _isInterruptible;

        public abstract SkillEffect ToSkillEffect();
    }
}
