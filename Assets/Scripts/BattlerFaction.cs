namespace AWE.Synzza {
    public readonly struct BattlerFaction {
        public const byte ID_NONE = 0;

        public byte ID { get; }
        public string DisplayName { get; }
        public bool IsFriendly { get; }
        public bool IsHostile { get; }

        public override bool Equals(object obj) => obj is BattlerFaction faction && Equals(faction);
        public bool Equals(in BattlerFaction other) => ID == other.ID;
        public override int GetHashCode() => ID;
        public override string ToString() => DisplayName;

        public BattlerFaction(byte id, string displayName, bool isFriendly, bool isHostile) {
            ID = id;
            DisplayName = displayName;
            IsFriendly = isFriendly;
            IsHostile = isHostile;
        }
    }

    public sealed class BattlerFactionRegistry : ByteSizedRegistry<BattlerFaction> {
        protected override byte GetRegisterableID(in BattlerFaction faction) => faction.ID;
        protected override string GetRegisterableDisplayName(in BattlerFaction faction) => faction.DisplayName;
        protected override bool TryGetInvalidID(out byte invalidID) { invalidID = BattlerFaction.ID_NONE; return true; }
    }
}
