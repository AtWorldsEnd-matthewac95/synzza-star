using System;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public class UnitySkillHitboxWorldObject : UnityWorldObject, ISkillHitboxWorldObject {
        public SkillHitbox Hitbox { get; }

        public UnitySkillHitboxWorldObject(SkillHitbox hitbox, MonoBehaviour mono, bool isMobile = true) : base(mono, isMobile) {
            Hitbox = hitbox;
        }
    }
}
