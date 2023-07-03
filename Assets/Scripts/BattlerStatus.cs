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

        public bool ApplyState(BattlerStatusState state) {
            if (state == BattlerStatusState.SkillWindDown && Current != BattlerStatusState.SkillEffect) {
                return false;
            }

            if (state == Current) {
                return true;
            }

            Current = state;

            OnBlockStatusChanged?.Invoke(isNowBlocking: Current == BattlerStatusState.Blocking);

            switch (Current) {

                case BattlerStatusState.OK:
                case BattlerStatusState.Blocking:
                    OnStationaryStatusChanged?.Invoke(isNowStationary: false);
                    break;

                case BattlerStatusState.Staggered:
                    OnStaggerApplied?.Invoke();
                    OnStationaryStatusChanged?.Invoke(isNowStationary: true);
                    break;

                case BattlerStatusState.SkillWindDown:
                    OnSkillWindDown?.Invoke();
                    OnStationaryStatusChanged?.Invoke(isNowStationary: true);
                    break;

                case BattlerStatusState.SkillEffect:
                case BattlerStatusState.SkillWindUp:
                    OnStationaryStatusChanged?.Invoke(isNowStationary: true);
                    break;

            }

            return true;
        }

        public bool RemoveState(BattlerStatusState stateToRemove) {
            if (stateToRemove == BattlerStatusState.OK) {
                return false;
            }

            bool isRemovingState = stateToRemove == Current;
            bool isRemovingBlock = Current == BattlerStatusState.Blocking;

            if (isRemovingState) {
                Current = BattlerStatusState.OK;
                OnStationaryStatusChanged?.Invoke(isNowStationary: false);

                if (isRemovingBlock) {
                    OnBlockStatusChanged?.Invoke(isNowBlocking: false);
                }
            }

            return isRemovingState;
        }
    }
}
