using System;

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

        public bool TryRegisterFaction(in BattlerFaction faction, bool isOverwritingAllowed = false) => TryRegister(faction, isOverwritingAllowed);
        public bool TryRegisterFaction(in BattlerFaction faction, out BattlerFaction registered) => TryRegister(faction, out registered);
        public void RegisterFaction(in BattlerFaction faction, bool isOverwritingAllowed = false) => Register(faction, isOverwritingAllowed);
        public bool UnregisterFaction(in BattlerFaction faction) => Unregister(faction);
    }
}
