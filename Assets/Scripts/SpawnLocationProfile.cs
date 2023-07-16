namespace AWE.Synzza {
    public readonly struct SpawnLocationProfile {
        public const byte INVALID_ID = 0;

        public byte ID { get; }
        public float3 PositionalOffset { get; }
        public float3 DirectionalOffsetMagnitudes { get; }
        public bool IsInheritingRotation { get; }
        public bool IsIgnoringInheritedXRotation { get; }
        public bool IsIgnoringInheritedYRotation { get; }
        public bool IsIgnoringInheritedZRotation { get; }

        public SpawnLocationProfile(byte id, in float3 positionalOffset, in float3 directionalOffsetMagnitudes, bool isInheritingRotation)
            : this(id, positionalOffset, directionalOffsetMagnitudes, isInheritingRotation, false, false, false) {}
        public SpawnLocationProfile(
            byte id,
            in float3 positionalOffset,
            in float3 directionalOffsetMagnitudes,
            bool isInheritingRotation,
            bool isIgnoringInheritedXRotation,
            bool isIgnoringInheritedYRotation,
            bool isIgnoringInheritedZRotation)
        {
            ID = id;
            PositionalOffset = positionalOffset;
            DirectionalOffsetMagnitudes = directionalOffsetMagnitudes;
            IsInheritingRotation = isInheritingRotation;
            IsIgnoringInheritedXRotation = isIgnoringInheritedXRotation;
            IsIgnoringInheritedYRotation = isIgnoringInheritedYRotation;
            IsIgnoringInheritedZRotation = isIgnoringInheritedZRotation;
        }

        public bool IsIgnoringAnyInheritedRotation => IsIgnoringInheritedXRotation || IsIgnoringInheritedYRotation || IsIgnoringInheritedZRotation;

        public float3 FindSpawnPosition(IWorldObject spawner)
            => spawner.WorldPosition
            + PositionalOffset
            + (DirectionalOffsetMagnitudes.x * spawner.WorldRight)
            + (DirectionalOffsetMagnitudes.y * spawner.WorldUp)
            + (DirectionalOffsetMagnitudes.z * spawner.WorldForward);

        public float3 FindSpawnPosition(IWorldObject spawner, in float3 positionalOffsetScale)
            => spawner.WorldPosition
            + new float3(PositionalOffset.x * positionalOffsetScale.x, PositionalOffset.y * positionalOffsetScale.y, PositionalOffset.z * positionalOffsetScale.z)
            + (DirectionalOffsetMagnitudes.x * spawner.WorldRight)
            + (DirectionalOffsetMagnitudes.y * spawner.WorldUp)
            + (DirectionalOffsetMagnitudes.z * spawner.WorldForward);

        public float3 FindSpawnPosition(IWorldObject spawner, in float3 positionalOffsetScale, in float3 directionalOffsetScale)
            => spawner.WorldPosition
            + new float3(PositionalOffset.x * positionalOffsetScale.x, PositionalOffset.y * positionalOffsetScale.y, PositionalOffset.z * positionalOffsetScale.z)
            + (DirectionalOffsetMagnitudes.x * directionalOffsetScale.x * spawner.WorldRight)
            + (DirectionalOffsetMagnitudes.y * directionalOffsetScale.y * spawner.WorldUp)
            + (DirectionalOffsetMagnitudes.z * directionalOffsetScale.z * spawner.WorldForward);

        public float3 FindSpawnPosition(IWorldObject spawner, float positionalOffsetScale)
            => spawner.WorldPosition
            + (PositionalOffset * positionalOffsetScale)
            + (DirectionalOffsetMagnitudes.x * spawner.WorldRight)
            + (DirectionalOffsetMagnitudes.y * spawner.WorldUp)
            + (DirectionalOffsetMagnitudes.z * spawner.WorldForward);

        public float3 FindSpawnPosition(IWorldObject spawner, float positionalOffsetScale, float directionalOffsetScale)
            => spawner.WorldPosition
            + (PositionalOffset * positionalOffsetScale)
            + (DirectionalOffsetMagnitudes.x * directionalOffsetScale * spawner.WorldRight)
            + (DirectionalOffsetMagnitudes.y * directionalOffsetScale * spawner.WorldUp)
            + (DirectionalOffsetMagnitudes.z * directionalOffsetScale * spawner.WorldForward);

        public float4 FindSpawnRotation(IWorldObject spawner) {
            var rotation = QuaternionMath.Calculator.identity;

            if (IsInheritingRotation) {
                if (IsIgnoringAnyInheritedRotation) {
                    var spawnEulerAngles = spawner.WorldRotation;
                    var eulerX = IsIgnoringInheritedXRotation ? 0f : spawnEulerAngles.x;
                    var eulerY = IsIgnoringInheritedYRotation ? 0f : spawnEulerAngles.y;
                    var eulerZ = IsIgnoringInheritedZRotation ? 0f : spawnEulerAngles.z;
                    QuaternionMath.Calculator.Euler(eulerX, eulerY, eulerZ);
                } else {
                    rotation = spawner.WorldQuaternion;
                }
            }

            return rotation;
        }
    }

    public sealed class SpawnLocationProfileRegistry : ByteSizedRegistry<SpawnLocationProfile> {
        protected override byte GetRegisterableID(in SpawnLocationProfile spawnProfile) => spawnProfile.ID;
        protected override bool TryGetInvalidID(out byte invalidID) { invalidID = SpawnLocationProfile.INVALID_ID; return true; }
    }
}
