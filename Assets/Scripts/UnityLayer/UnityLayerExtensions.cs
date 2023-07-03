using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public static class UnityLayerExtensions {
        public static float3 ToFloat3(this in Vector3 vector) => new(vector.x, vector.y, vector.z);
        public static float4 ToFloat4(this in Quaternion quaternion) => new(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        public static Vector3 ToVector3(this in float3 f3) => new(f3.x, f3.y, f3.z);
        public static Quaternion ToQuaternion(this in float4 f4) => new(f4.x, f4.y, f4.z, f4.w);
    }
}
