using TMPro;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class BattlerStatusUI : MonoBehaviour {
        public Camera WorldGameCamera;
        public BattlerWorldObject BattlerWorldObject;

        private TextMeshProUGUI _text;

        private void Start() {
            Debug.Assert(BattlerWorldObject != null, $"{typeof(BattlerStatusUI).Name} \"{gameObject.name}\" does not have a {typeof(BattlerWorldObject).Name} instance assigned!");

            if (BattlerWorldObject == null) {
                Destroy(gameObject);
            }

            _text = GetComponent<TextMeshProUGUI>();

            if (WorldGameCamera == null) {
                Debug.LogWarning($"{typeof(BattlerStatusUI).Name} \"{gameObject.name}\" does not have a {typeof(Camera).Name} instance assigned! Finding the value in the scene...");
                WorldGameCamera = Camera.main;
            }
        }

        private void Update() {
            var status = BattlerWorldObject.Battler.Status.Current;
            _text.text = status.ToString();
            var screenPoint = (2f * WorldGameCamera.WorldToViewportPoint(BattlerWorldObject.WorldPosition.ToVector3())) - Vector3.one;
            transform.localPosition = new Vector3(screenPoint.x * transform.parent.position.x, screenPoint.y * transform.parent.position.y);
        }
    }
}
