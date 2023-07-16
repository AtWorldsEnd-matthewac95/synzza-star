using System;
using System.Collections.Generic;

namespace AWE.Synzza {
    public readonly struct SkillCooldownProfile {
        public bool IsUsingInnateCooldown { get; }
        public uint Cooldown { get; }

        public SkillCooldownProfile(bool isUsingInnate, uint cooldown) {
            IsUsingInnateCooldown = isUsingInnate;
            Cooldown = cooldown;
        }
    }

    public sealed class Skill {
        public int ID { get; }
        public string DisplayName { get; }
        public SkillEffect Effect { get; }
        public DurationProfile<float> WindUpSeconds { get; }
        public DurationProfile<float> WindDownSeconds { get; }

        private readonly bool _isUsingInnateCooldown;
        private readonly uint _cooldown;
        public uint GetCooldown(Battler source) => _isUsingInnateCooldown ? source.InnateSkillCooldown : _cooldown;

        public Skill(
            int id,
            string displayName,
            SkillEffect effect,
            in SkillCooldownProfile cooldownProfile,
            in DurationProfile<float> windUpSeconds,
            in DurationProfile<float> windDownSeconds = default
        ) {
            ID = id;
            DisplayName = displayName;
            Effect = effect;
            _isUsingInnateCooldown = cooldownProfile.IsUsingInnateCooldown;
            _cooldown = cooldownProfile.Cooldown;
            WindUpSeconds = windUpSeconds;
            WindDownSeconds = windDownSeconds;
        }

        public IEnumerable<ICoWait> CreateCoroutine(IBattlerWorldObject source, IBattlerWorldObject target) {
            if (!source.TrySetCurrentSkillUsage(new SkillUsage(this, Effect.CreateUsage(this, source, target, WindDownSeconds), target))) {
                yield break;
            }

            var status = source.Battler.Status;
            status.ApplyState(BattlerStatusState.SkillWindUp);

            yield return new CoWaitForSeconds(WindDownSeconds.Duration);

            if (!SkillUsage.IsUsageStale(source.CurrentSkillUsage)) {
                status.ApplyState(BattlerStatusState.SkillEffect);
                source.StartCoroutine(source.CurrentSkillUsage.EffectUsage.Coroutine);
            }
        }

        public override bool Equals(object obj) => obj is Skill skill && skill.ID == ID;
        public override int GetHashCode() => ID;
        public override string ToString() => $"{GetType().Name} {ID}: {DisplayName}";
    }

    public sealed class SkillUsage {
        public Skill Skill { get; }
        public SkillEffectUsage EffectUsage { get; }
        public IBattlerWorldObject Target { get; private set; }
        public bool IsStale => EffectUsage?.IsStale ?? true || Target == null;

        public SkillUsage(Skill skill, SkillEffectUsage effectUsage, IBattlerWorldObject target) {
            Skill = skill;
            EffectUsage = effectUsage;
            Target = target;
            Target.OnPreDestroy += (preDestroyObject) => {
                if (preDestroyObject == Target) {
                    Target = null;
                }
            };
        }

        public static bool IsUsageStale(SkillUsage usage) => usage?.IsStale ?? true;
    }

    public sealed class SkillRegistry {
        private readonly Dictionary<int, Skill> _registry = new();

        public bool IsRegistered(int id) => _registry.ContainsKey(id);
        public Skill this[int id] => _registry[id];

        public bool TryRegisterSkill(Skill skill, bool isOverwritingAllowed = false) => TryRegisterSkill(skill, out _, isOverwritingAllowed);
        public bool TryRegisterSkill(Skill skill, out Skill registered) => TryRegisterSkill(skill, out registered, false);
        private bool TryRegisterSkill(Skill skill, out Skill registered, bool isOverwritingAllowed) {
            var id = skill.ID;
            var isAbleToRegister = isOverwritingAllowed || !IsRegistered(id);

            if (isAbleToRegister) {
                UncheckedRegisterSkill(skill);
            }

            registered = _registry[id];
            return isAbleToRegister;
        }

        public void RegisterSkill(Skill skill, bool isOverwritingAllowed = false) {
            var id = skill.ID;
            if (!isOverwritingAllowed && IsRegistered(id)) {
                throw new InvalidOperationException($"Overwriting existing registered skill \"{_registry[id].DisplayName}\" with ID {id} is disallowed.");
            }
            UncheckedRegisterSkill(skill);
        }

        private void UncheckedRegisterSkill(Skill skill) {
            _registry.Add(skill.ID, skill);
        }

        public bool Unregister(Skill skill) => _registry.Remove(skill.ID);
    }
}
