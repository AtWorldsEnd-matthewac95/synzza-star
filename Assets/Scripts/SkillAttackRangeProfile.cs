namespace AWE.Synzza {
    public readonly struct SkillAttackRangeProfile {
        public const byte INVALID_ID = 0;

        public byte ID { get; }
        public FloatRange Range { get; }

        public override bool Equals(object obj) => obj is SkillAttackRangeProfile profile && Equals(profile);
        public bool Equals(in SkillAttackRangeProfile other) => ID == other.ID;
        public override int GetHashCode() => ID;
        public override string ToString() => $"{GetType().Name}_{ID}: ({Range.Min}, {Range.Max})";

        public SkillAttackRangeProfile(byte id, in FloatRange range) {
            ID = id;
            Range = range;
        }
    }

    public sealed class SkillAttackRangeProfileRegistry : ByteSizedRegistry<SkillAttackRangeProfile> {
        protected override byte GetRegisterableID(in SkillAttackRangeProfile registerable) => registerable.ID;
        protected override bool TryGetInvalidID(out byte invalidID) { invalidID = SkillAttackRangeProfile.INVALID_ID; return true; }
    }
}
