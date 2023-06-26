namespace AWE.Synzza {
    public struct float3 {
        public float x { get; }
        public float y { get; }
        public float z { get; }

        public float3(float X, float Y, float Z) { x = X; y = Y; z = Z; }
    }

    public struct float4 {
        public float x { get; }
        public float y { get; }
        public float z { get; }
        public float w { get; }

        public float4(float X, float Y, float Z, float W) { x = X; y = Y; z = Z; w = W; }
    }

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
    }
}