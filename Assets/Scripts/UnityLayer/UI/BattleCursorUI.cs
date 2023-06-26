using UnityEngine;
using UnityEngine.UI;

namespace AWE.Synzza.UnityLayer.Monocomponents.UI {
    public class BattleCursorUI : MonoBehaviour {
        public enum Orientation : byte {
            Down,
            Up,
            Left,
            Right
        }

        [SerializeField] private Image _uiImage;
        [SerializeField] private UiCursorAnchorMonocomponent _target;
        [SerializeField] private Camera _camera;

        public Orientation CurrentOrientation { get; private set; }

        private bool IsPointInViewport(Vector3 point) => IsPointInViewport(point.x, point.y);
        private bool IsPointInViewport(float x, float y) => x > 0f && y > 0f && x < _camera.pixelWidth && y < _camera.pixelHeight;

        private float OrientationToRotation(Orientation value) => value switch {
            Orientation.Right => 90f,
            Orientation.Up => 180f,
            Orientation.Left => 270f,
            _ => 0f,
        };

        private void Update() {
            var isTopValid = IsPointInViewport(_target.CursorScreenTop);
            CurrentOrientation = isTopValid ? Orientation.Down : Orientation.Up;
            _uiImage.transform.SetLocalPositionAndRotation(isTopValid ? _target.CursorScreenTop : _target.CursorScreenBottom, Quaternion.Euler(0f, 0f, OrientationToRotation(CurrentOrientation)));
        }
    }
}
