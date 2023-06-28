using TMPro;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Monocomponents {
    public class CameraFollowMonocomponent : MonoBehaviour {
        [SerializeField] private Transform _followThis;

        private Vector3 _offset;

        private void Awake() {
            _offset = transform.localPosition;
        }

        private void Update() {
            transform.position = _offset + _followThis.position;
        }
    }
}
