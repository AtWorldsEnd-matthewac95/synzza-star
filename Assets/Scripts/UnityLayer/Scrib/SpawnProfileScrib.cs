using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    [CreateAssetMenu(fileName = "NewSpawnProfile", menuName = "Scrib/SpawnProfile")]
    public class SpawnProfileScrib : ScriptableObject {
        [Tooltip("World positional offset from the spawner. This value is agnostic of the spawner's rotation.")]
        [SerializeField] private Vector3 _spawnerPositionalOffset;
        [Tooltip("NOT A POSITION. When the object is spawned, it should be spawned at (spawner.position + positionalOffset + this.X * spawner.right + this.Y * spawner.up + this.Z * spawner.forward)")]
        [SerializeField] private Vector3 _spawnerDirectionalOffsetMagnitudes;
        [Tooltip("Should the object copy the spawner's rotation when spawned?")]
        [SerializeField] private bool _isInheritingSpawnerRotation;
        [Tooltip("If the spawner's rotation is being inherited, should the X rotation be ignored?")]
        [SerializeField] private bool _isIgnoringInheritedXRotation;
        [Tooltip("If the spawner's rotation is being inherited, should the Y rotation be ignored?")]
        [SerializeField] private bool _isIgnoringInheritedYRotation;
        [Tooltip("If the spawner's rotation is being inherited, should the Z rotation be ignored?")]
        [SerializeField] private bool _isIgnoringInheritedZRotation;

        public Vector3 SpawnerPositionalOffset => _spawnerPositionalOffset;
        public Vector3 SpawnerDirectionalOffsetMagnitudes => _spawnerDirectionalOffsetMagnitudes;
        public bool IsInheritingSpawnerRotation => _isInheritingSpawnerRotation;
        public bool IsIgnoringInheritedXRotation => _isIgnoringInheritedXRotation;
        public bool IsIgnoringInheritedYRotation => _isIgnoringInheritedYRotation;
        public bool IsIgnoringInheritedZRotation => _isIgnoringInheritedZRotation;
        public bool IsIgnoringAnyInheritedRotation => _isIgnoringInheritedXRotation || _isIgnoringInheritedYRotation || _isIgnoringInheritedZRotation;

        public Vector3 FindSpawnPosition(Transform spawner)
            => spawner.position
            + _spawnerPositionalOffset
            + (_spawnerDirectionalOffsetMagnitudes.x * spawner.right)
            + (_spawnerDirectionalOffsetMagnitudes.y * spawner.up)
            + (_spawnerDirectionalOffsetMagnitudes.z * spawner.forward);

        public Vector3 FindSpawnPosition(Transform spawner, in Vector3 positionalOffsetScale)
            => spawner.position
            + new Vector3(_spawnerPositionalOffset.x * positionalOffsetScale.x, _spawnerPositionalOffset.y * positionalOffsetScale.y, _spawnerPositionalOffset.z * positionalOffsetScale.z)
            + (_spawnerDirectionalOffsetMagnitudes.x * spawner.right)
            + (_spawnerDirectionalOffsetMagnitudes.y * spawner.up)
            + (_spawnerDirectionalOffsetMagnitudes.z * spawner.forward);

        public Vector3 FindSpawnPosition(Transform spawner, in Vector3 positionalOffsetScale, in Vector3 directionalOffsetScale)
            => spawner.position
            + new Vector3(_spawnerPositionalOffset.x * positionalOffsetScale.x, _spawnerPositionalOffset.y * positionalOffsetScale.y, _spawnerPositionalOffset.z * positionalOffsetScale.z)
            + (_spawnerDirectionalOffsetMagnitudes.x * directionalOffsetScale.x * spawner.right)
            + (_spawnerDirectionalOffsetMagnitudes.y * directionalOffsetScale.y * spawner.up)
            + (_spawnerDirectionalOffsetMagnitudes.z * directionalOffsetScale.z * spawner.forward);

        public Vector3 FindSpawnPosition(Transform spawner, float positionalOffsetScale)
            => spawner.position
            + (_spawnerPositionalOffset * positionalOffsetScale)
            + (_spawnerDirectionalOffsetMagnitudes.x * spawner.right)
            + (_spawnerDirectionalOffsetMagnitudes.y * spawner.up)
            + (_spawnerDirectionalOffsetMagnitudes.z * spawner.forward);

        public Vector3 FindSpawnPosition(Transform spawner, float positionalOffsetScale, float directionalOffsetScale)
            => spawner.position
            + (_spawnerPositionalOffset * positionalOffsetScale)
            + (_spawnerDirectionalOffsetMagnitudes.x * directionalOffsetScale * spawner.right)
            + (_spawnerDirectionalOffsetMagnitudes.y * directionalOffsetScale * spawner.up)
            + (_spawnerDirectionalOffsetMagnitudes.z * directionalOffsetScale * spawner.forward);
    }
}
