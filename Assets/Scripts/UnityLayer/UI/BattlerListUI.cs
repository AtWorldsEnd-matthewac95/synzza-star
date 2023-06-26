using System.Text;
using TMPro;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Monocomponents.UI {
    public class BattlerListUI : MonoBehaviour {
        [Tooltip("Text UI element for listing the friendly battlers.")]
        [SerializeField] private TextMeshProUGUI _playerListText;
        [Tooltip("Text UI element for listing the enemy battlers.")]
        [SerializeField] private TextMeshProUGUI _enemyListText;

        private void Start() {
            StringBuilder text = new();

            if (_playerListText == null) {
                Debug.LogError($"{GetType().Name} {gameObject.name} has not been assigned a {typeof(TextMeshProUGUI).Name} instance for listing player display names!");
            } else {
                var players = FindObjectsOfType<PlayerBattlerMonocomponent>();

                foreach (var player in players) {
                    text.Append($"{player.DisplayName}\n");
                }

                _playerListText.text = text.ToString();
                text.Clear();
            }

            if (_enemyListText == null) {
                Debug.LogError($"{GetType().Name} {gameObject.name} has not been assigned a {typeof(TextMeshProUGUI).Name} instance for listing enemy display names!");
            } else {
                var enemies = FindObjectsOfType<EnemyBattlerMonocomponent>();

                foreach (var enemy in enemies) {
                    text.Append($"{enemy.DisplayName}\n");
                }

                _enemyListText.text = text.ToString();
                text.Clear();
            }
        }
    }
}
