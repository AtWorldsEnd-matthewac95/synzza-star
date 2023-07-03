using System.Collections.Generic;

namespace AWE.Synzza {
    public interface ISceneObject {
        float3 LocalPosition { get; }
        float3 WorldPosition { get; }
        float3 LocalRotation { get; }
        float3 WorldRotation { get; }
        float4 LocalQuaternion { get; }
        float4 WorldQuaternion { get; }
        float3 LocalScale { get; }
        float3 WorldScale { get; }
        bool IsMobile { get; }
        float3 WorldForward { get; }
        float3 WorldRight { get; }
        float3 WorldUp { get; }

        void StartCoroutine(in IEnumerator<ICoWait> coroutine);
    }

    public interface IBattlerSceneObject : ISceneObject {
        Battler Battler { get; }
        SkillUsage CurrentSkillUsage { get; }

        bool TrySetCurrentSkillUsage(SkillUsage usage);
    }
}