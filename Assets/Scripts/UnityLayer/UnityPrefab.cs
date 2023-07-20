using System;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public class UnityPrefab : IWorldObjectTemplate {
        public GameObject Prefab { get; }

        public UnityPrefab(GameObject prefab) { Prefab = prefab; }

        public Volume CalculateSpawnVolume(in float3 rotation) => default;
    }

    public class UnitySkillHitboxPrefab : UnityPrefab, ISkillHitboxWorldObjectTemplate {
        public SkillHitbox SkillHitbox { get; protected set; }
        public IBattlerWorldObject SourceBattler { get; protected set; }

        public UnitySkillHitboxPrefab(GameObject prefab, SkillHitbox hitbox = null) : base(prefab) {
            SkillHitbox = hitbox;
        }

        public void SetSkillHitbox(SkillHitbox hitbox) {
            if (SkillHitbox != null) {
                throw new InvalidOperationException($"Cannot overwrite existing {typeof(SkillHitbox).Name}!");
            }

            SkillHitbox = hitbox;
        }

        public void SetSourceBattler(IBattlerWorldObject sourceBattler) {
            if (SourceBattler != null) {
                throw new InvalidOperationException($"Cannot overwrite existing {SourceBattler.GetType().Name}!");
            }

            SourceBattler = sourceBattler;
        }

        public void ClearSkillHitbox() => SkillHitbox = null;
        public void ClearSourceBattler() => SourceBattler = null;

        public UnitySkillHitboxPrefab Clone() => new(Prefab, SkillHitbox);
        ISkillHitboxWorldObjectTemplate ISkillHitboxWorldObjectTemplate.Clone() => Clone();
    }
}
