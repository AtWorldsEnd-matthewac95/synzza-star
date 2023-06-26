namespace AWE.Synzza {
    public enum BattlerStatusState : byte {
        OK,
        SkillWindUp,
        SkillEffect,
        SkillWindDown,
        Staggered,
        Blocking
    }

    public class BattlerStatus {
        public BattlerStatusState MainState { get; set; }
        public bool IsVulnerable => MainState == BattlerStatusState.SkillWindUp || MainState == BattlerStatusState.SkillWindDown || MainState == BattlerStatusState.Staggered;

        public BattlerStatus() => MainState = BattlerStatusState.OK;
    }
}
