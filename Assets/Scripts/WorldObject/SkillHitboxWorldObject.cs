namespace AWE.Synzza {
    public interface ISkillHitboxWorldObject : IMutableWorldObject {
        SkillHitbox Hitbox { get; }
    }

    public class SkillHitboxWorldObject : WorldObject, ISkillHitboxWorldObject {
        public SkillHitbox Hitbox { get; }

        public SkillHitboxWorldObject(Impl impl, SkillHitbox hitbox) : base(impl) {
            Hitbox = hitbox;
        }
    }
}
