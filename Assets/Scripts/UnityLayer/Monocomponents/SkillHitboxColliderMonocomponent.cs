using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    // TODO - Remove from UnityLayer

    [RequireComponent(typeof(Collider))]
    public class SkillHitboxColliderMonocomponent : MonoBehaviour {
        private bool _isInitialized = false;

        public bool IsInitialized => _isInitialized;
        public SkillHitboxMonocomponent Parent { get; private set; }

        public void Initialize(SkillHitboxMonocomponent parent) {
            if (_isInitialized) {
                Debug.LogError($"{GetType().Name} {gameObject.name} attempted to initialize more than once!");
                return;
            }

            Parent = parent;
            _isInitialized = true;
        }
    }
}
