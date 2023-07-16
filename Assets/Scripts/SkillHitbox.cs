namespace AWE.Synzza {
    public sealed class SkillHitbox {
        public Battler SourceBattler { get; }
        public Skill SourceSkill { get; }
        public bool IsFriendlyFire { get; }

        public SkillHitbox(Battler sourceBattler, Skill sourceSkill, bool isFriendlyFire) {
            SourceBattler = sourceBattler;
            SourceSkill = sourceSkill;
            IsFriendlyFire = isFriendlyFire;
        }
    }
}
