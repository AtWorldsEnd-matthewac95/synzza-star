using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [CreateAssetMenu(fileName = "NewSkillCooldownProfile", menuName = "Scrib/SkillCooldownProfile")]
    public class SkillCooldownProfileScrib : ScriptableObject {
        [SerializeField] private bool _isUsingInnateCooldownTurns;
        [SerializeField] [Min(1)] private uint _cooldownTurns = 1;

        public SkillCooldownProfile ToSkillCooldownProfile() => new(_isUsingInnateCooldownTurns, _cooldownTurns);
    }
}
