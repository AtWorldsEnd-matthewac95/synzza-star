using System;
using System.Reflection;

namespace AWE.Synzza {
    public enum BattlerMeleeRules : byte {
        AutoBlock,
        AutoAttack,
        AutoCounter,
        OpportuneAttack
    }

    public class Battler {
        public byte FactionID { get; private set; }

        public void SetFaction(in BattlerFaction faction) => SetFaction(faction.ID, faction.DisplayName);
        public void SetFaction(byte factionId) => SetFaction(factionId, string.Empty);
        private void SetFaction(byte factionId, string factionName) {
            if (!SynzzaGame.Current.BattlerFactions.IsRegistered(factionId)) {
                var factionNameString = string.IsNullOrWhiteSpace(factionName) ? string.Empty : $" \"{factionName}\"";
                throw new ArgumentException($"{GetType().Name} \"{DisplayName}\" attempted to set its faction to an unregistered faction{factionNameString}!");
            }

            FactionID = factionId;
        }

        public string DisplayName { get; private set; }
        public uint InnateSkillCooldown { get; private set; }
        public float InnateMeleeAttackRange { get; private set; }
        public BattlerMeleeRules InnateMeleeRules { get; }
        public BattlerStatus Status { get; }
        public BattlerStaggerProfile StaggerProfile { get; }

        public delegate void MeleeRulesUpdateDelegate(bool isSameRules, BattlerMeleeRules newRules);
        public event MeleeRulesUpdateDelegate OnUpdateMeleeRules;

        private BattlerMeleeRules _currentMeleeRules;
        public BattlerMeleeRules CurrentMeleeRules {
            get => _currentMeleeRules;
            set {
                bool isSameState = _currentMeleeRules == value;
                _currentMeleeRules = value;
                OnUpdateMeleeRules?.Invoke(isSameState, _currentMeleeRules);
            }
        }

        public event SkillEffectCancelledDelegate OnSkillEffectCancelled;

        public Battler(string displayName, uint innateSkillCooldown, float innateMeleeAttackRange, BattlerStaggerProfile staggerProfile, BattlerMeleeRules innateMeleeRules = BattlerMeleeRules.AutoBlock) {
            DisplayName = displayName;
            InnateSkillCooldown = Math.Max(innateSkillCooldown, 1);
            InnateMeleeAttackRange = Math.Max(innateMeleeAttackRange, 0f);
            InnateMeleeRules = innateMeleeRules;
            CurrentMeleeRules = InnateMeleeRules;
            StaggerProfile = staggerProfile;

            Status = new();
            Status.OnStaggerApplied += OnStaggerApplied;

            FactionID = BattlerFaction.ID_NONE;
        }

        private void OnStaggerApplied() {
            if (Status.Current != BattlerStatusState.Staggered) {
                throw new InvalidOperationException($"{GetType().Name} \"{DisplayName}\" attempted to run {MethodBase.GetCurrentMethod().Name} when its status was {Status.Current}!");
            }

            OnSkillEffectCancelled?.Invoke(BattlerStatusState.Staggered);
        }
    }
}
