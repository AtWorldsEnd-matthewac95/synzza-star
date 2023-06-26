using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    [CreateAssetMenu(fileName = "NewBattler", menuName = "Scrib/Battler")]
    public class BattlerScrib : ScriptableObject {
        [SerializeField] private string _displayName;
        [SerializeField] [Min(1)] private int _defaultSkillCooldown = 1;
        [SerializeField] private float _defaultMeleeAttackRange;
        [SerializeField] private BattlerContinuousState _defaultMeleeState;
        [SerializeField] private BattlerContinuousState _defaultRangeState;

        public string DisplayName => _displayName;
        public int DefaultSkillCooldown => _defaultSkillCooldown;
        public float DefaultMeleeAttackRange => _defaultMeleeAttackRange;
        public BattlerContinuousState DefaultMeleeState => _defaultMeleeState;
        public BattlerContinuousState DefaultRangeState => _defaultRangeState;

        public Battler ToBattler(BattlerTeam team) => new(team, _displayName, _defaultSkillCooldown, _defaultMeleeAttackRange, _defaultMeleeState, _defaultRangeState);
    }
}
