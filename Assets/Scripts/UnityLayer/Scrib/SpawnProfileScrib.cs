using UnityEngine;

namespace AWE.Synzza.UnityLayer {
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

        public SpawnProfile ToSpawnProfile(byte id) => new(
            id,
            _spawnerPositionalOffset.ToFloat3(),
            _spawnerDirectionalOffsetMagnitudes.ToFloat3(),
            _isInheritingSpawnerRotation,
            _isIgnoringInheritedXRotation,
            _isIgnoringInheritedYRotation,
            _isIgnoringInheritedZRotation
        );
    }
}
