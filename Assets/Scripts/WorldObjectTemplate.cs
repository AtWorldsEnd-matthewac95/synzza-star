using System;

namespace AWE.Synzza {
    public interface IWorldObjectTemplate {
        Volume CalculateSpawnVolume(in float3 rotation);
    }

    public interface IBattlerWorldObjectTemplate : IWorldObjectTemplate {
        Battler Battler { get; }
    }

    public interface ISkillHitboxWorldObjectTemplate : IWorldObjectTemplate {
        SkillHitbox SkillHitbox { get; }
        IBattlerWorldObject SourceBattler { get; }

        // TODO - This can be better... maybe just a single Initialize/PrepareForSpawn method? Only, not sure if all platforms will need the source battler world object...

        void SetSkillHitbox(SkillHitbox hitbox);
        void ClearSkillHitbox();

        void SetSourceBattler(IBattlerWorldObject sourceBattler);
        void ClearSourceBattler();

        ISkillHitboxWorldObjectTemplate Clone();
    }
}
