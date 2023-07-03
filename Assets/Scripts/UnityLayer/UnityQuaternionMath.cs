using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public sealed class UnityQuaternionMath : QuaternionMath {
        public override float4 Euler(float x, float y, float z) => Quaternion.Euler(x, y, z).ToFloat4();
        public override float4 identity => Quaternion.identity.ToFloat4();
    }
}
