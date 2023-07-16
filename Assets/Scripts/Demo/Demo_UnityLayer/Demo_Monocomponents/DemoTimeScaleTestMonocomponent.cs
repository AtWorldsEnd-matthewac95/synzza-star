using UnityEngine;

namespace AWE.Synzza.Demo.UnityLayer {
    public class DemoTimeScaleTestMonocomponent : MonoBehaviour {
        [SerializeField] [Min(0f)] private float _timeScale = 1f;
        [SerializeField] private bool _resetTimeScale;
        [SerializeField] private float _speed = 3f;
        [SerializeField] private float _radius = 3f;

        private int _currentFrameCount = 0;
        private Vector3 _origin;
        private float _originalTimeScale;
        private float _radians = 0f;

        private void Awake() {
            _origin = transform.position;
            _originalTimeScale = Time.timeScale;
        }

        private void Update() {
            ++_currentFrameCount;

            if (_resetTimeScale) {
                _currentFrameCount = 0;
                _resetTimeScale = false;
                Time.timeScale = _originalTimeScale;
                _timeScale = _originalTimeScale;
            } else if (Mathf.Abs(Time.timeScale - _timeScale) > 0.001f) {
                _currentFrameCount = 0;
                Time.timeScale = _timeScale;
            }

            if (_currentFrameCount % 30 == 0) {
                Debug.Log($"Current time scale: {_timeScale}, Total number of frames while at this time scale: {_currentFrameCount}");
            }

            _radians += _speed * Time.deltaTime;
            transform.position = _radius * new Vector3(Mathf.Cos(_radians), Mathf.Sin(_radians));
        }
    }
}
