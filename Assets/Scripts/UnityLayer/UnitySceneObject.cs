using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public static partial class UnityLayerExtensions {
        public static float3 ToFloat3(this in Vector3 vector) => new(vector.x, vector.y, vector.z);
        public static float4 ToFloat4(this in Quaternion quaternion) => new(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        public static Vector3 ToVector3(this in float3 f3) => new(f3.x, f3.y, f3.z);
        public static Quaternion ToQuaternion(this in float4 f4) => new(f4.x, f4.y, f4.z, f4.w);
    }

    public class UnitySceneObject : ISceneObject {
        public Transform transform { get; }
        public bool IsMobile { get; set; }

        public UnitySceneObject(Transform T, bool isMobile = true) { transform = T; IsMobile = isMobile; }

        public float3 LocalPosition => transform.localPosition.ToFloat3();
        public float3 WorldPosition => transform.position.ToFloat3();
        public float3 LocalRotation => transform.localEulerAngles.ToFloat3();
        public float3 WorldRotation => transform.eulerAngles.ToFloat3();
        public float4 LocalQuaternion => transform.localRotation.ToFloat4();
        public float4 WorldQuaternion => transform.rotation.ToFloat4();
        public float3 LocalScale => transform.localScale.ToFloat3();
        public float3 WorldScale => transform.lossyScale.ToFloat3();
    }
}
