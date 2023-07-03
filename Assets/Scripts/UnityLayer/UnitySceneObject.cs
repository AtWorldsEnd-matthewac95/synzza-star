using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public class UnitySceneObject : ISceneObject {
        public Transform transform { get; }
        public bool IsMobile { get; set; }

        private MonoBehaviour _mono;

        public UnitySceneObject(Transform T, bool isMobile = true) {
            transform = T;
            IsMobile = isMobile;

            if (!transform.gameObject.TryGetComponent(out _mono)) {
                _mono = null;
            }
        }
        public UnitySceneObject(MonoBehaviour mono, bool isMobile = true) {
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
    }

    public class UnityBattlerSceneObject : UnitySceneObject, IBattlerSceneObject {
        public Battler Battler { get; }
        public SkillUsage CurrentSkillUsage { get; private set; }

        public UnityBattlerSceneObject(Battler battler, Transform T, bool isMobile = true) : base(T, isMobile) {
            Battler = battler;
            CurrentSkillUsage = null;
        }
        public UnityBattlerSceneObject(BattlerMonocomponent battler, bool isMobile = true) : base(battler, isMobile) {
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
    }
}
