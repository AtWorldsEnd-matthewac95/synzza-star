namespace AWE.Synzza {
    public enum BattlerStatusState : byte {
        None,
        OK,
        SkillWindUp,
        SkillEffect,
        SkillWindDown,
        Staggered,
        Blocking
    }

    public static class BattlerStatusStateExtensions {
        public static bool IsStationaryState(this BattlerStatusState statusState) => statusState switch {
            BattlerStatusState.SkillWindUp => true,
            BattlerStatusState.SkillEffect => true,
            BattlerStatusState.SkillWindDown => true,
            BattlerStatusState.Staggered => true,
            _ => false
        };
    }

    public class BattlerStatus {
        public BattlerStatusState Current { get; private set; }
        public bool IsVulnerable => Current == BattlerStatusState.SkillWindUp || Current == BattlerStatusState.SkillWindDown || Current == BattlerStatusState.Staggered;

        public event StaggerAppliedDelegate OnStaggerApplied;
        public event SkillWindDownDelegate OnSkillWindDown;
        public event BlockStatusChangedDelegate OnBlockStatusChanged;
        public event StationaryStatusChangedDelegate OnStationaryStatusChanged;

        public BattlerStatus() {
            Current = BattlerStatusState.OK;
        }

        public bool ApplyState(BattlerStatusState state, bool? stationaryStatusOverride = null) {
            if (state == BattlerStatusState.SkillWindDown && Current != BattlerStatusState.SkillEffect) {
                return false;
            }

            if (state == Current) {
                return true;
            }

            var isNewStateStationary = stationaryStatusOverride ?? state.IsStationaryState();
            var isStationaryStatusChanging = Current.IsStationaryState() != state.IsStationaryState();

            Current = state;

            OnBlockStatusChanged?.Invoke(isNowBlocking: Current == BattlerStatusState.Blocking);

            if (isStationaryStatusChanging) {
                OnStationaryStatusChanged?.Invoke(isNewStateStationary);
            }

            switch (Current) {
                case BattlerStatusState.Staggered:
                    OnStaggerApplied?.Invoke();
                    break;

                case BattlerStatusState.SkillWindDown:
                    OnSkillWindDown?.Invoke();
                    break;
            }

            return true;
        }

        public bool RemoveState(BattlerStatusState stateToRemove) {
            if (stateToRemove == BattlerStatusState.OK) {
                return false;
            }

            bool isRemovingState = stateToRemove == Current;
            if (isRemovingState) {
                bool isRemovingBlock = Current == BattlerStatusState.Blocking;
                bool isRemovingStationary = Current.IsStationaryState();

                Current = BattlerStatusState.OK;

                if (isRemovingBlock) {
                    OnBlockStatusChanged?.Invoke(isNowBlocking: false);
                }

                if (isRemovingStationary) {
                    OnStationaryStatusChanged?.Invoke(isNowStationary: false);
                }
            }

            return isRemovingState;
        }
    }
}
