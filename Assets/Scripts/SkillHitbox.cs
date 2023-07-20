namespace AWE.Synzza {
    public sealed class SkillHitbox {
        public IBattlerWorldObject SourceBattler { get; }
        public Skill SourceSkill { get; }
        public bool IsFriendlyFire { get; }

        public SkillHitbox(IBattlerWorldObject sourceBattler, Skill sourceSkill, bool isFriendlyFire) {
            SourceBattler = sourceBattler;
            SourceSkill = sourceSkill;
            IsFriendlyFire = isFriendlyFire;
        }
    }
}
