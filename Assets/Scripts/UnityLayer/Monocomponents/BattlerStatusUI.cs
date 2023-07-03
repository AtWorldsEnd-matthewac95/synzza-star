using TMPro;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class BattlerStatusUI : MonoBehaviour {
        public Camera WorldGameCamera;
        public BattlerMonocomponent BattlerMono;

        private TextMeshProUGUI _text;

        private void Start() {
            Debug.Assert(BattlerMono != null, $"{typeof(BattlerStatusUI).Name} \"{gameObject.name}\" does not have a {typeof(BattlerMonocomponent).Name} instance assigned!");

            if (BattlerMono == null) {
                Destroy(gameObject);
            }

            _text = GetComponent<TextMeshProUGUI>();

            if (WorldGameCamera == null) {
                Debug.LogWarning($"{typeof(BattlerStatusUI).Name} \"{gameObject.name}\" does not have a {typeof(Camera).Name} instance assigned! Finding the value in the scene...");
                WorldGameCamera = Camera.main;
            }
        }

        private void Update() {
            var status = BattlerMono.Battler.Status.Current;
            _text.text = status.ToString();
            var screenPoint = (2f * WorldGameCamera.WorldToViewportPoint(BattlerMono.transform.position)) - Vector3.one;
            transform.localPosition = new Vector3(screenPoint.x * transform.parent.position.x, screenPoint.y * transform.parent.position.y);
        }
    }
}
