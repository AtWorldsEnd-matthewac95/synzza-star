using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    [CreateAssetMenu(fileName = "NewSkillCooldownProfile", menuName = "Scrib/SkillCooldownProfile")]
    public class SkillCooldownProfileScrib : ScriptableObject {
        [SerializeField] private bool _isUsingDefaultCooldownTurns;
        [SerializeField] [Min(1)] private int _cooldownTurns = 1;

        public bool IsUsingDefaultCooldownTurns => _isUsingDefaultCooldownTurns;
        public int CooldownTurns => _cooldownTurns;
    }
}
