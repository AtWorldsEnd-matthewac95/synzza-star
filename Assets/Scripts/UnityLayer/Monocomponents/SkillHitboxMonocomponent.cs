using Unity.VisualScripting;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public class SkillHitboxMonocomponent : MonoBehaviour {
        private bool _isInitialized = false;

        public bool IsInitialized => _isInitialized;
        public BattlerMonocomponent SourceBattler { get; private set; }

        public void Initialize(BattlerMonocomponent source) {
            if (_isInitialized) {
                Debug.LogError($"{GetType().Name} \"{gameObject.name}\" attempted to be initialized more than once!");
                return;
            }

            SourceBattler = source;
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
