using System;

namespace AWE.Synzza {
    public enum GameInitializationStage : byte {
        Created = 0,
        BattlerFactionRegistration = 1,
        BattlerFactionRegistrationCompleted = 3,
        SpawnProfileRegistration = 5,
        SpawnProfileRegistrationCompleted = 8,
        SkillRegistration = 10,
        SkillRegistrationCompleted = 11,
        Done = byte.MaxValue
    }

    public sealed class SynzzaGame {
        private static SynzzaGame _current = null;

        public static void CreateGame() {
            if (_current != null) {
                throw new InvalidOperationException($"Cannot overwrite existing {typeof(SynzzaGame).Name} instance!");
            }

            _current = new();
        }

        public static void DestroyGame() {
            if (_current == null) {
                throw new InvalidOperationException($"No existing {typeof(SynzzaGame).Name} instance to destroy!");
            }

            _current = null;
        }

        public static SynzzaGame Current => _current;

        public GameInitializationStage InitializationStage { get; private set; }

        private GameInitializationStage PeniultimateInitializationStage => GameInitializationStage.SkillRegistrationCompleted;

        public BattlerFactionRegistry BattlerFactions { get; }
        public SpawnProfileRegistry SpawnProfiles { get; }
        public SkillRegistry Skills { get; }

        private SynzzaGame() {
            BattlerFactions = new();
            SpawnProfiles = new();
            Skills = new();
            InitializationStage = GameInitializationStage.Created;
        }

        private bool IsCompletionInitializationStage(GameInitializationStage stage) => stage switch {
            GameInitializationStage.Created
                or GameInitializationStage.BattlerFactionRegistrationCompleted
                or GameInitializationStage.SpawnProfileRegistrationCompleted
                or GameInitializationStage.SkillRegistrationCompleted
                or GameInitializationStage.Done => true,
            _ => false,
        };

        private GameInitializationStage GetCompletionInitializationStage(GameInitializationStage stage) => stage switch {
            GameInitializationStage.BattlerFactionRegistration => GameInitializationStage.BattlerFactionRegistrationCompleted,
            GameInitializationStage.SpawnProfileRegistration => GameInitializationStage.SpawnProfileRegistrationCompleted,
            GameInitializationStage.SkillRegistration => GameInitializationStage.SkillRegistrationCompleted,
            _ => throw new ArgumentException($"{stage} has no completion stage!")
        };

        public void BeginInitializationStage(GameInitializationStage stage) {
            if (IsCompletionInitializationStage(stage)) {
                throw new ArgumentException($"{typeof(GameInitializationStage).Name}.{stage} is not valid for this operation!");
            }

            if (InitializationStage >= stage) {
                throw new InvalidOperationException($"This {GetType().Name} instance has moved past or is on the \"{stage}\" {typeof(GameInitializationStage).Name}.");
            }

            InitializationStage = stage;
        }

        public void CompleteInitializationStage(GameInitializationStage completionStage) {
            if (completionStage != GetCompletionInitializationStage(InitializationStage)) {
                throw new InvalidOperationException($"{typeof(GameInitializationStage).Name}.{completionStage} is not valid for this operation!");
            }

            InitializationStage = completionStage;
        }

        public void SetInitializationDone() {
            if (InitializationStage != PeniultimateInitializationStage) {
                throw new InvalidOperationException($"This {GetType().Name} instance has not yet completed initialization!");
            }

            InitializationStage = GameInitializationStage.Done;
        }
    }
}
