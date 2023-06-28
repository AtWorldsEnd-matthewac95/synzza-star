using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Scrib/Skill")]
    public class SkillScrib : ScriptableObject {
        [SerializeField] private string _displayName;
        [SerializeField] [Min(0f)] private float _windUpDurationSeconds;
        [SerializeField] [Min(0f)] private float _windUpVarianceSeconds;
        [SerializeField] [Min(0f)] private float _windDownDurationSeconds;
        [SerializeField] private SkillEffectScrib _effect;
        [SerializeField] private SkillDisplayProfileScrib _displayProfile;
        [SerializeField] private SkillCooldownProfileScrib _cooldownProfile;

        public string DisplayName => _displayName;
        public float WindUpDurationSeconds => _windUpDurationSeconds;
        public float WindUpVarianceSeconds => _windUpVarianceSeconds;
        public float WindDownDurationSeconds => _windDownDurationSeconds;
        public SkillEffectScrib Effect => _effect;
        public SkillDisplayProfileScrib DisplayProfile => _displayProfile;
        public SkillCooldownProfileScrib CooldownProfile => _cooldownProfile;
    }
}
