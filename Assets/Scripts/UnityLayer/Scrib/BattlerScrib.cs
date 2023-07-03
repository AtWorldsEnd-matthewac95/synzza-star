using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [CreateAssetMenu(fileName = "NewBattler", menuName = "Scrib/Battler")]
    public class BattlerScrib : ScriptableObject {
        [SerializeField] private string _displayName;
        [SerializeField] [Min(1)] private uint _innateSkillCooldown = 1;
        [SerializeField] private float _innateMeleeAttackRange;
        [SerializeField] private BattlerMeleeRules _innateMeleeRules;
        [SerializeField] private BattlerStaggerProfileScrib _staggerProfile;

        public string DisplayName => _displayName;
        public uint InnateSkillCooldown => _innateSkillCooldown;
        public float InnateMeleeAttackRange => _innateMeleeAttackRange;
        public BattlerMeleeRules InnateMeleeState => _innateMeleeRules;
        public BattlerStaggerProfileScrib StaggerProfile => _staggerProfile;

        public Battler ToBattler() {
            BattlerStaggerProfile staggerProfile;

            if (_staggerProfile == null) {
                Debug.LogWarning($"{GetType().Name} {name} has no {typeof(BattlerStaggerProfileScrib).Name} instance! Creating a default {typeof(BattlerStaggerProfileScrib).Name}.");
                staggerProfile = new(0f);
            } else {
                staggerProfile = _staggerProfile.ToBattlerStaggerProfile();
            }

            return new(_displayName, _innateSkillCooldown, _innateMeleeAttackRange, staggerProfile, _innateMeleeRules);
        }
    }
}
