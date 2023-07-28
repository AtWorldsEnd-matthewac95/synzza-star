using AWE.Synzza.UnityLayer;
using System;
using UnityEngine;

namespace AWE.Synzza {
    public delegate void PlayerSelectedTargetBattlerDelegate(BattlerWorldObject target);

    public class PlayerBattleInputUI : MonoBehaviour {
        [SerializeField] private CameraFollowMonocomponent _cameraFollow;

        private BattlerWorldObject _assignedPlayer;

        private int _enemyIndex = 0;
        private bool _isSelecting = false;
        private BattlerWorldObject _currentEnemy = null;
        private UnityWorld _world = null;
        private float _defaultTimeScale;

        public bool IsSelecting => _isSelecting;
        public BattlerWorldObject CurrentEnemy => _currentEnemy;

        public event PlayerSelectedTargetBattlerDelegate OnPlayerSelectedTargetBattler;

        private void Awake() {
            _defaultTimeScale = Time.timeScale;
        }

        private void Start() {
            if (SingletonSynzzaGame.Current?.GetCurrentWorld() is not UnityWorld world) {
                Debug.LogError($"{GetType().Name} does not function if the {typeof(SingletonSynzzaGame).Name} instance does not have a {typeof(UnityWorld).Name} world.");
                UnityEngine.Object.Destroy(gameObject);
            } else {
                _world = world;
            }
        }

        private void Update() {
            if (!_isSelecting) {
                if (Input.GetKeyDown(KeyCode.P)) {
                    _assignedPlayer.Battler.ResetTargetBattlerToNull();
                    BeginSelection();
                }
                return;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                MoveToNextEnemy();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
                MoveToPreviousEnemy();
            }
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space)) {
                ConfirmSelection();
            }
        }

        public void AssignPlayer(BattlerWorldObject player) {
            if (player == null) {
                return;
            }

            if (_assignedPlayer != null) {
                throw new InvalidOperationException($"Cannot assign player {player.Battler.DisplayName} to {GetType().Name} {gameObject.name} because it already has an assigned player {_assignedPlayer.Battler.DisplayName}!");
            }

            _assignedPlayer = player;
            _assignedPlayer.OnPlayerBattlerNeedsInput += OnPlayerBattlerNeedsInput;
        }

        private void OnPlayerBattlerNeedsInput(in IBattlerWorldObject player) {
            if (player == _assignedPlayer) {
                BeginSelection();
            }
        }

        public void BeginSelection() {
            if (!_isSelecting) {
                _isSelecting = true;
                _enemyIndex = 0;
                Time.timeScale = 0f;
                FocusOnEnemy();
            }
        }

        private void FocusOnEnemy() {
            if (!_world.Battlers.TryFindBattler(
                out BattlerWorldObject foundBattler,
                (byte)BattlerFactionID.Enemies,
                (in IBattlerWorldObject battler, int index) => { return index == _enemyIndex; }
            )) {
                Debug.LogWarning($"{GetType().Name} {gameObject.name} failed to find a battler in {BattlerFactionID.Enemies} with an index of {_enemyIndex}!");
                return;
            }

            if (foundBattler is not BattlerWorldObject enemy) {
                Debug.LogError($"{GetType().Name} {gameObject.name} found a battler in {BattlerFactionID.Enemies} with an index of {_enemyIndex}, but it was not a {typeof(BattlerWorldObject).Name} instance!");
                return;
            }

            var enemyTransform = enemy.GetImpl<UnityWorldObject>();

            _currentEnemy = enemy;
            _cameraFollow.SetCurrentFocus(enemyTransform.transform);
        }

        private void ConfirmSelection() {
            if (_isSelecting) {
                OnPlayerSelectedTargetBattler?.Invoke(_currentEnemy);
            }
        }

        public void EndSelection() {
            if (_isSelecting && _currentEnemy != null) {
                _isSelecting = false;
                _cameraFollow.ResetCurrentFocus();
                Time.timeScale = _defaultTimeScale;
                _currentEnemy = null;
            }
        }

        public void MoveToNextEnemy() {
            ++_enemyIndex;
            _enemyIndex = _enemyIndex >= _world.Battlers.FindBattlerCountWithFaction((byte)BattlerFactionID.Enemies) ? 0 : _enemyIndex;
            FocusOnEnemy();
        }

        public void MoveToPreviousEnemy() {
            --_enemyIndex;
            _enemyIndex = _enemyIndex < 0 ? _world.Battlers.FindBattlerCountWithFaction((byte)BattlerFactionID.Enemies) - 1 : _enemyIndex;
            FocusOnEnemy();
        }
    }
}
