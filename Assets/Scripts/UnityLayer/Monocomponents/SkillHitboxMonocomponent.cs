using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    // TODO - Remove from UnityLayer

    public class SkillHitboxMonocomponent : MonoBehaviour {
        private bool _isInitialized = false;

        public bool IsInitialized => _isInitialized;
        public UnitySkillHitboxWorldObject WorldObject { get; private set; }

        public void Initialize(SkillHitbox skillHitbox) {
            if (_isInitialized) {
                Debug.LogError($"{GetType().Name} \"{gameObject.name}\" attempted to be initialized more than once!");
                return;
            }

            WorldObject = new(skillHitbox, this);
            _isInitialized = true;
        }

        private void Awake() {
            var children = gameObject.GetComponentsInChildren<Collider>(includeInactive: true);

            foreach (var child in children) {
                if (!child.gameObject.TryGetComponent(out SkillHitboxColliderMonocomponent colliderMono)) {
                    colliderMono = child.gameObject.AddComponent<SkillHitboxColliderMonocomponent>();
                }

                if (!colliderMono.IsInitialized) {
                    colliderMono.Initialize(this);
                } else if (colliderMono.Parent != this) {
                    var parentName = colliderMono.Parent == null ? "null" : colliderMono.Parent.gameObject.name;
                    Debug.LogError($"{GetType().Name} \"{gameObject.name}\" contains a collider \"{colliderMono.gameObject.name}\" whose parent appears to be set incorrectly to \"{parentName}\".");
                }
            }
        }
    }
}
