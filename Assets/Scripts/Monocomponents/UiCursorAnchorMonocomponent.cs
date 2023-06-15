using UnityEngine;

namespace AWE.Synzza.Monocomponents {
    public class UiCursorAnchorMonocomponent : MonoBehaviour {
        public Camera WorldGameCamera;

        [SerializeField] private float _rightWorldOffset;
        [SerializeField] private float _topWorldOffset;
        [SerializeField] private float _forwardWorldOffset;
        [SerializeField] private float _leftWorldOffset;
        [SerializeField] private float _bottomWorldOffset;
        [SerializeField] private float _backwardWorldOffset;

        public Vector3 CursorWorldRight => transform.position + (Vector3.right * _rightWorldOffset);
        public Vector3 CursorWorldTop => transform.position + (Vector3.up * _topWorldOffset);
        public Vector3 CursorWorldForward => transform.position + (Vector3.forward * _forwardWorldOffset);
        public Vector3 CursorWorldLeft => transform.position + (Vector3.left * _leftWorldOffset);
        public Vector3 CursorWorldBottom => transform.position + (Vector3.down * _bottomWorldOffset);
        public Vector3 CursorWorldBackward => transform.position + (Vector3.back * _backwardWorldOffset);

        public Vector3 CursorScreenRight => WorldGameCamera.WorldToScreenPoint(CursorWorldRight);
        public Vector3 CursorScreenTop => WorldGameCamera.WorldToScreenPoint(CursorWorldTop);
        public Vector3 CursorScreenForward => WorldGameCamera.WorldToScreenPoint(CursorWorldForward);
        public Vector3 CursorScreenLeft => WorldGameCamera.WorldToScreenPoint(CursorWorldLeft);
        public Vector3 CursorScreenBottom => WorldGameCamera.WorldToScreenPoint(CursorWorldBottom);
        public Vector3 CursorScreenBackward => WorldGameCamera.WorldToScreenPoint(CursorWorldBackward);

        private void Awake() {
            if (WorldGameCamera == null) {
                Debug.LogError($"{GetType().Name} {gameObject.name} has no value set for World Game Camera!");
                WorldGameCamera = Camera.main;
            }
        }
    }
}
