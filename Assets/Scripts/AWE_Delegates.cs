using System.Collections;

namespace AWE.Synzza {
    public delegate void SkillEffectCancelledDelegate(BattlerStatusState cancellingStatus);
    public delegate void StaggerAppliedDelegate();
    public delegate void SkillWindDownDelegate();
    public delegate void BlockStatusChangedDelegate(bool isNowBlocking);
    public delegate void StationaryStatusChangedDelegate(bool isNowStationary);
    public delegate void WorldObjectPreDestroyDelegate(in IMutableWorldObject preDestroyObject);
    public delegate void BattleEndDelegate(Battle battle);
    public delegate void WorldExitDelegate();
    public delegate void DestroyGameDelegate(SynzzaGame destroyedGame);
    public delegate void ChangeTargetBattlerDelegate(in IBattlerWorldObject oldTarget, in IBattlerWorldObject newTarget);
    public delegate void ChangeTargetSkillDelegate(Skill oldSkill, Skill newSkill);
    public delegate void CoroutineFinishedDelegate(in IEnumerator coroutine);
    public delegate void BattlerStatusChangedDelegate(BattlerStatusState oldState, BattlerStatusState newState);
    public delegate void PlayerBattlerNeedsInputDelegate(in IBattlerWorldObject playerBattler);
    public delegate bool ForEachBattlerWorldObjectEarlyExitDelegate(in IBattlerWorldObject battler, int index);
    public delegate bool WorldObjectEqualityDelegate(in IMutableWorldObject lhs, in IMutableWorldObject rhs);
    public delegate void AddBattlerToWorldCatalogDelegate(in IBattlerWorldObject battler);
    public delegate void RemoveBattlerFromWorldCatalogDelegate(in IBattlerWorldObject battler);
    public delegate void BattleDelayedStartDelegate(Battle battle);
}
