namespace AWE.Synzza {
    public enum BattlerTeam : byte {
        Enemies = 0,
        Players
    }

    public class Battler {
        public BattlerTeam Team { get; }
        public string DisplayName { get; }
        public int DefaultSkillCooldown { get; }
        public float DefaultMeleeAttackRange { get; }
        public BattlerContinuousState DefaultMeleeState { get; }
        public BattlerContinuousState DefaultRangeState { get; }

        private readonly BattlerStatus _status = new();

        public BattlerStatusState CurrentState => _status.MainState;
        public bool IsVulnerable => _status.IsVulnerable;

        public BattlerContinuousState CurrentContinuousState { get; private set; }

        public Battler(
            BattlerTeam team,
            string displayName,
            int defaultSkillCooldown,
            float defaultMeleeAttackRange,
            BattlerContinuousState defaultMeleeState = BattlerContinuousState.AutoBlock,
            BattlerContinuousState defaultRangeState = BattlerContinuousState.AutoBlock
        ) {
            Team = team;
            DisplayName = displayName;
            DefaultSkillCooldown = defaultSkillCooldown;
            DefaultMeleeAttackRange = defaultMeleeAttackRange;
            DefaultMeleeState = defaultMeleeState;
            DefaultRangeState = defaultRangeState;
        }
    }
}
