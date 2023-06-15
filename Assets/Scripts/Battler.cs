namespace AWE.Synzza {
    public enum BattlerTeam : byte {
        Enemies = 0,
        Players
    }

    public class Battler {
        public BattlerTeam Team { get; }
        public string DisplayName { get; }

        public Battler(BattlerTeam team, string displayName) {
            Team = team;
            DisplayName = displayName;
        }
    }
}
