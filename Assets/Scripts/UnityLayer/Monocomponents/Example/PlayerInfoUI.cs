using TMPro;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Example {
    public class PlayerInfoUI : MonoBehaviour {
        [SerializeField] private PlayerNavAgentMonocomponent _player;
        [SerializeField] private PlayerBattleInputUI _input;
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _playerMeleeRules;
        [SerializeField] private TextMeshProUGUI _playerTarget;

        private void Start() {
            if (_player == null) {
                Debug.LogError($"{GetType().Name} {gameObject.name} has no {typeof(PlayerNavAgentMonocomponent).Name} reference!");
                UnityEngine.GameObject.Destroy(gameObject);
            }
            if (_input == null) {
                Debug.LogError($"{GetType().Name} {gameObject.name} has no {typeof(PlayerBattleInputUI).Name} reference!");
                UnityEngine.GameObject.Destroy(gameObject);
            }
        }

        private void Update() {
            _playerName.text = _player.WorldObject.Battler.DisplayName;
            _playerMeleeRules.text = _player.WorldObject.Battler.CurrentMeleeRules.ToString();
            _playerTarget.text = _player.WorldObject.Battler.CurrentTargetBattler?.Battler.DisplayName ?? _input.CurrentEnemy?.Battler.DisplayName ?? string.Empty;
        }
    }
}
