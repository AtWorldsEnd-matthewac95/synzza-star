using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public class CameraFollowMonocomponent : MonoBehaviour {
        [SerializeField] private Transform _followThis;

        private Vector3 _offset;

        public Transform CurrentFocus { get; private set; }

        private void Awake() {
            _offset = transform.localPosition;
            CurrentFocus = _followThis;
        }

        private void Update() {
            transform.position = _offset + CurrentFocus.position;
        }

        public void SetCurrentFocus(Transform newFocus) => CurrentFocus = newFocus;
        public void ResetCurrentFocus() => CurrentFocus = _followThis;
    }
}
