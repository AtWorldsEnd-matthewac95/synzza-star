using System;
using System.Reflection;

namespace AWE.Synzza {
    public enum BattlerMeleeRules : byte {
        AutoBlock,
        AutoAttack,
        AutoCounter,
        OpportuneAttack
    }

    public class Battler : SynzzaGameDependent {
        public byte FactionID { get; private set; }
        public bool IsPlayerBattler => FactionID == _game.PlayerFactionID;

        public void SetFaction(in BattlerFaction faction) => SetFaction(faction.ID, faction.DisplayName);
        public void SetFaction(byte factionId) => SetFaction(factionId, string.Empty);
        private void SetFaction(byte factionId, string factionName) {
            if (!_game.BattlerFactions.IsRegistered(factionId)) {
                var factionNameString = string.IsNullOrWhiteSpace(factionName) ? string.Empty : $" \"{factionName}\"";
                throw new ArgumentException($"{GetType().Name} \"{DisplayName}\" attempted to set its faction to an unregistered faction{factionNameString}!");
            }

            FactionID = factionId;
        }

        public string DisplayName { get; protected set; }
        public uint InnateSkillCooldown { get; protected set; }
        public float InnateMeleeAttackRange { get; protected set; }
        public BattlerMeleeRules InnateMeleeRules { get; }
        public BattlerStatus Status { get; }
        public BattlerStaggerProfile StaggerProfile { get; }

        public delegate void MeleeRulesUpdateDelegate(bool isSameRules, BattlerMeleeRules newRules);
        public event MeleeRulesUpdateDelegate OnUpdateMeleeRules;

        protected BattlerMeleeRules _currentMeleeRules;
        public BattlerMeleeRules CurrentMeleeRules {
            get => _currentMeleeRules;
            set {
                bool isSameState = _currentMeleeRules == value;
                _currentMeleeRules = value;
                OnUpdateMeleeRules?.Invoke(isSameState, _currentMeleeRules);
            }
        }

        public event SkillEffectCancelledDelegate OnSkillEffectCancelled;
        public event ChangeTargetBattlerDelegate OnChangeTargetBattler;
        public event ChangeTargetSkillDelegate OnChangeTargetSkill;

        public Battle CurrentBattle { get; protected set; }
        public IBattlerWorldObject CurrentTargetBattler { get; protected set; }
        public IBattlerWorldObject CurrentBlockTargetBattler { get; protected set; }
        public Skill CurrentBlockSkill { get; protected set; }
        public Skill CurrentTargetSkill { get; protected set; }
        public Skill CurrentCounterSkill { get; protected set; }
        public Skill DefaultCounterSkill { get; protected set; }

        public Battler(
            SynzzaGame game,
            string displayName,
            uint innateSkillCooldown,
            float innateMeleeAttackRange,
            BattlerStaggerProfile staggerProfile,
            int blockSkillID,
            int defaultCounterSkillID,
            BattlerMeleeRules innateMeleeRules = BattlerMeleeRules.AutoBlock
        ) : base(game) {
            DisplayName = displayName;
            InnateSkillCooldown = Math.Max(innateSkillCooldown, 1);
            InnateMeleeAttackRange = Math.Max(innateMeleeAttackRange, 0f);
            InnateMeleeRules = innateMeleeRules;
            _currentMeleeRules = InnateMeleeRules;
            StaggerProfile = staggerProfile;

            SetBlockSkill(blockSkillID);
            SetDefaultCounterSkill(defaultCounterSkillID);
            CurrentCounterSkill = DefaultCounterSkill;

            Status = new();
            Status.OnStaggerApplied += OnStaggerApplied;
            Status.OnSkillWindDown += OnSkillWindDown;

            FactionID = BattlerFaction.ID_NONE;
            CurrentBattle = null;
        }

        protected virtual void OnStaggerApplied() {
            if (Status.Current != BattlerStatusState.Staggered) {
                throw new InvalidOperationException($"{GetType().Name} \"{DisplayName}\" attempted to run {MethodBase.GetCurrentMethod().Name} when its status was {Status.Current}!");
            }

            OnSkillEffectCancelled?.Invoke(BattlerStatusState.Staggered);
            ResetTargetBattlerToNull();
        }

        protected virtual void OnSkillWindDown() {
            if (Status.Current != BattlerStatusState.SkillWindDown) {
                throw new InvalidOperationException($"{GetType().Name} \"{DisplayName}\" attempted to run {MethodBase.GetCurrentMethod().Name} when its status was {Status.Current}!");
            }

            ResetTargetBattlerToNull();
        }

        public void SetBlockSkill(int blockSkillID) {
            if (!_game.Skills.TryGetSkill(blockSkillID, out var blockSkill)) {
                throw new ArgumentException($"Skill {blockSkillID} is not registered!");
            }

            if (blockSkill.Effect is not BlockSkillEffect) {
                throw new ArgumentException($"{blockSkill} does not have a block skill effect!");
            }

            CurrentBlockSkill = blockSkill;
        }

        protected void SetDefaultCounterSkill(int defaultCounterSkillID) {
            if (!_game.Skills.IsRegistered(defaultCounterSkillID)) {
                throw new ArgumentException($"Skill {defaultCounterSkillID} is not registered!");
            }

            DefaultCounterSkill = _game.Skills[defaultCounterSkillID];
        }

        public void SetCounterSkill(int counterSkillID) {
            if (!_game.Skills.IsRegistered(counterSkillID)) {
                throw new ArgumentException($"Skill {counterSkillID} is not registered!");
            }

            CurrentCounterSkill = _game.Skills[counterSkillID];
        }

        public void ResetCounterSkillToDefault() {
            CurrentCounterSkill = DefaultCounterSkill;
        }

        public void SetTargetBattler(in IBattlerWorldObject battler) {
            var previousTarget = CurrentTargetBattler;
            CurrentTargetBattler = battler;

            OnChangeTargetBattler?.Invoke(previousTarget, CurrentTargetBattler);
        }

        public void ResetTargetBattlerToNull() {
            var previousTarget = CurrentTargetBattler;
            CurrentTargetBattler = null;

            OnChangeTargetBattler?.Invoke(previousTarget, CurrentTargetBattler);
        }

        public void SetTargetSkill(int skillID) {
            if (!_game.Skills.IsRegistered(skillID)) {
                throw new ArgumentException($"Skill {skillID} is not registered!");
            }

            var previousSkill = CurrentTargetSkill;
            CurrentTargetSkill = _game.Skills[skillID];

            OnChangeTargetSkill?.Invoke(previousSkill, CurrentTargetSkill);
        }

        public void ResetTargetSkillToNull() {
            var previousSkill = CurrentTargetSkill;
            CurrentTargetSkill = null;

            OnChangeTargetSkill?.Invoke(previousSkill, CurrentTargetSkill);
        }

        public void SetBlockTargetBattler(in IBattlerWorldObject battler) => CurrentBlockTargetBattler = battler;
        public void ResetBlockTargetBattlerToNull() => CurrentBlockTargetBattler = null;
    }
}
