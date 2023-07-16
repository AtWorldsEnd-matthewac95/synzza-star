namespace AWE.Synzza {
    public delegate void SkillEffectCancelledDelegate(BattlerStatusState cancellingStatus);
    public delegate void StaggerAppliedDelegate();
    public delegate void SkillWindDownDelegate();
    public delegate void BlockStatusChangedDelegate(bool isNowBlocking);
    public delegate void StationaryStatusChangedDelegate(bool isNowStationary);
    public delegate void SceneObjectPreDestroyDelegate(IWorldObject preDestroyObject);
    public delegate void BattleEndDelegate(Battle battle);
    public delegate void WorldExitDelegate();
    public delegate void DestroyGameDelegate(SynzzaGame destroyedGame);
}
