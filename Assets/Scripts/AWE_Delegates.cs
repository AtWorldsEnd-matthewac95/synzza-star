namespace AWE.Synzza {
    public delegate void SkillEffectCancelledDelegate(BattlerStatusState cancellingStatus);
    public delegate void StaggerAppliedDelegate();
    public delegate void SkillWindDownDelegate();
    public delegate void BlockStatusChangedDelegate(bool isNowBlocking);
    public delegate void StationaryStatusChangedDelegate(bool isNowStationary);
}
