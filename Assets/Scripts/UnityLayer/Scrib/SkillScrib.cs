using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Scrib/Skill")]
    public class SkillScrib : ScriptableObject {
        [SerializeField] private int _id;
        [SerializeField] private string _displayName;
        [SerializeField] [Min(0f)] private float _windUpDurationSeconds;
        [SerializeField] [Min(0f)] private float _windUpVarianceSeconds;
        [SerializeField] [Min(0f)] private float _windDownDurationSeconds;
        [SerializeField] private SkillEffectScrib _effect;
        [SerializeField] private SkillDisplayProfileScrib _displayProfile;
        [SerializeField] private SkillCooldownProfileScrib _cooldownProfile;

        public int ID => _id;

        public Skill ToSkill() => new(_id, _displayName, _effect.ToSkillEffect(), _cooldownProfile.ToSkillCooldownProfile(), new(_windUpDurationSeconds, _windUpVarianceSeconds), new(_windDownDurationSeconds));
    }
}
