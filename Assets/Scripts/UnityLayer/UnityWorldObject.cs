using System;
using System.Collections.Generic;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public class UnityWorldObject : IWorldObject {
        public Transform transform { get; }
        public bool IsMobile { get; set; }

        private readonly MonoBehaviour _mono;

        public UnityWorldObject(Transform T, bool isMobile = true) {
            transform = T;
            IsMobile = isMobile;

            if (!transform.gameObject.TryGetComponent(out _mono)) {
                _mono = null;
            }
        }
        public UnityWorldObject(MonoBehaviour mono, bool isMobile = true) {
            _mono = mono;
            transform = _mono.transform;
            IsMobile = isMobile;
        }

        public float3 LocalPosition => transform.localPosition.ToFloat3();
        public float3 WorldPosition => transform.position.ToFloat3();
        public float3 LocalRotation => transform.localEulerAngles.ToFloat3();
        public float3 WorldRotation => transform.eulerAngles.ToFloat3();
        public float4 LocalQuaternion => transform.localRotation.ToFloat4();
        public float4 WorldQuaternion => transform.rotation.ToFloat4();
        public float3 LocalScale => transform.localScale.ToFloat3();
        public float3 WorldScale => transform.lossyScale.ToFloat3();
        public float3 WorldForward => transform.forward.ToFloat3();
        public float3 WorldRight => transform.right.ToFloat3();
        public float3 WorldUp => transform.up.ToFloat3();

        public void StartCoroutine(in IEnumerator<ICoWait> coroutine) {
            if (_mono != null) {
                _mono.StartCoroutine(new UnityCoroutine(coroutine));
            }
        }

        public void StartCoroutine(in IEnumerable<ICoWait> coroutine) {
            if (_mono != null) {
                _mono.StartCoroutine(new UnityCoroutine(coroutine));
            }
        }

        public event SceneObjectPreDestroyDelegate OnPreDestroy;

        public void Destroy() {
            OnPreDestroy?.Invoke(this);
            UnityEngine.Object.Destroy(transform.gameObject);
        }
    }

    public class UnityBattlerWorldObject : UnityWorldObject, IBattlerWorldObject {
        public Battler Battler { get; }
        public SkillUsage CurrentSkillUsage { get; private set; }

        public UnityBattlerWorldObject(Battler battler, Transform T, bool isMobile = true) : base(T, isMobile) {
            Battler = battler;
            CurrentSkillUsage = null;
        }
        public UnityBattlerWorldObject(BattlerMonocomponent battler, bool isMobile = true) : base(battler, isMobile) {
            Battler = battler.Battler;
            CurrentSkillUsage = null;
        }

        public bool TrySetCurrentSkillUsage(SkillUsage usage) {
            if (!CurrentSkillUsage?.IsStale ?? false) {
                if (CurrentSkillUsage.EffectUsage.IsInterruptible) {
                    CurrentSkillUsage.Skill.Effect.InterruptEffect(this, CurrentSkillUsage.Target, CurrentSkillUsage.EffectUsage.Capture);
                } else {
                    return false;
                }
            }

            CurrentSkillUsage = usage;
            return true;
        }

        public void ReactToSkillHitbox(ISkillHitboxWorldObject hitbox) {
            //var sourceBattler = hitbox.Hitbox.
        }
    }

    public class UnitySkillHitboxWorldObject : UnityWorldObject, ISkillHitboxWorldObject {
        public SkillHitbox Hitbox { get; }

        // TODO - Do we really need BattlerMonocomponent? Seems like IBattlerWorldObject would be sufficient...

        public UnitySkillHitboxWorldObject(SkillHitbox hitbox, BattlerMonocomponent sourceBattler, Transform T, bool isMobile = true) : base(T, isMobile) {
            Hitbox = hitbox;

            if (T.gameObject.TryGetComponent(out SkillHitboxMonocomponent mono)) {
                if (sourceBattler == null) {
                    throw new ArgumentException($"Cannot initialize a {mono.GetType().Name} component with a null {typeof(BattlerMonocomponent).Name}!");
                }

                mono.Initialize(sourceBattler);
            } else {
                throw new ArgumentException($"{GetType().Name} must be created with a {T.GetType().Name} attached to a {T.gameObject.GetType().Name} with a {typeof(SkillHitboxMonocomponent).Name} component!");
            }
        }
    }
}
