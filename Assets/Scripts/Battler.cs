using System;

namespace AWE.Synzza {
    public enum BattlerTeam : byte {
        Enemies = 0,
        Players
    }

    public class Battler {
        public BattlerTeam Team { get; }
        public string DisplayName { get; }
        public int DefaultSkillCooldown { get; }
        public float DefaultMeleeAttackRange { get; }
        public BattlerContinuousState DefaultMeleeState { get; }
        public BattlerContinuousState DefaultRangeState { get; }

        private readonly BattlerStatus _status = new();

        public BattlerStatusState CurrentStatus => _status.MainState;
        public bool IsVulnerable => _status.IsVulnerable;

        public delegate void ContinuousStateUpdateDelegate(bool isSameState, BattlerContinuousState newState);
        public event ContinuousStateUpdateDelegate OnUpdateContinuousState;

        private BattlerContinuousState _currentContinuousState;
        public BattlerContinuousState CurrentContinuousState {
            get => _currentContinuousState;
            set {
                bool isSameState = _currentContinuousState == value;
                _currentContinuousState = value;
                OnUpdateContinuousState?.Invoke(isSameState, _currentContinuousState);
            }
        }

        public delegate void CancelEffectsDelegate();
        public event CancelEffectsDelegate OnCancelEffects;

        public delegate void WindDownEffectDelegate();
        public event WindDownEffectDelegate OnWindDown;

        public delegate void BlockStatusChangedDelegate(bool isNowBlocking);
        public event BlockStatusChangedDelegate OnBlockStatusChanged;

        public delegate void StationaryStatusChangedDelegate(bool isNowStationary);
        public event StationaryStatusChangedDelegate OnStationaryStatusChanged;

        public Battler(
            BattlerTeam team,
            string displayName,
            int defaultSkillCooldown,
            float defaultMeleeAttackRange,
            BattlerContinuousState defaultMeleeState = BattlerContinuousState.AutoBlock,
            BattlerContinuousState defaultRangeState = BattlerContinuousState.AutoBlock
        ) {
            Team = team;
            DisplayName = displayName;
            DefaultSkillCooldown = defaultSkillCooldown;
            DefaultMeleeAttackRange = defaultMeleeAttackRange;
            DefaultMeleeState = defaultMeleeState;
            DefaultRangeState = defaultRangeState;
        }

        public bool ApplyStatusState(BattlerStatusState state) {
            if (state == BattlerStatusState.SkillWindDown && _status.MainState != BattlerStatusState.SkillEffect) {
                return false;
            }

            if (state == _status.MainState) {
                return true;
            }

            _status.MainState = state;

            OnBlockStatusChanged?.Invoke(isNowBlocking: _status.MainState == BattlerStatusState.Blocking);

            switch (_status.MainState) {

                case BattlerStatusState.OK:
                case BattlerStatusState.Blocking:
                    OnStationaryStatusChanged?.Invoke(isNowStationary: false);
                    break;

                case BattlerStatusState.Staggered:
                    OnCancelEffects?.Invoke();
                    OnStationaryStatusChanged?.Invoke(isNowStationary: true);
                    break;

                case BattlerStatusState.SkillWindDown:
                    OnWindDown?.Invoke();
                    OnStationaryStatusChanged?.Invoke(isNowStationary: true);
                    break;

                case BattlerStatusState.SkillEffect:
                case BattlerStatusState.SkillWindUp:
                    OnStationaryStatusChanged?.Invoke(isNowStationary: true);
                    break;

            }

            return true;
        }

        public bool RemoveStatusState(BattlerStatusState stateToRemove) {
            if (stateToRemove == BattlerStatusState.OK) {
                return false;
            }

            bool isRemovingState = stateToRemove == _status.MainState;
            bool isRemovingBlock = _status.MainState == BattlerStatusState.Blocking;

            if (isRemovingState) {
                _status.MainState = BattlerStatusState.OK;
                OnStationaryStatusChanged?.Invoke(isNowStationary: false);

                if (isRemovingBlock) {
                    OnBlockStatusChanged?.Invoke(isNowBlocking: false);
                }
            }

            return isRemovingState;
        }
    }
}
