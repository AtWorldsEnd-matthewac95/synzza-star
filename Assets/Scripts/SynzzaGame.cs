using System;

namespace AWE.Synzza {
    public enum SingletonGameInitializationStage : byte {
        Created = 0,
        BattlerFactionRegistration = 1,
        BattlerFactionRegistrationCompleted = 3,
        SpawnLocationProfileRegistration = 5,
        SpawnLocationProfileRegistrationCompleted = 8,
        SkillRegistration = 10,
        SkillRegistrationCompleted = 11,
        SkillAttackRangeProfileRegistration = 12,
        SkillAttackRangeProfileRegistrationCompleted = 14,
        Done = byte.MaxValue
    }

    public abstract class SynzzaGame {
        public abstract BattlerFactionRegistry BattlerFactions { get; }
        public abstract SpawnLocationProfileRegistry SpawnLocations { get; }
        public abstract SkillRegistry Skills { get; }
        public abstract SkillAttackRangeProfileRegistry SkillAttackRanges { get; }
        public abstract BattleCoordinator Battles { get; }
        public abstract byte PlayerFactionID { get; }

        public abstract IWorld GetCurrentWorld();
        public abstract void SetCurrentWorld(IWorld world);

        public event DestroyGameDelegate OnDestroyGame;

        protected void InvokeOnDestroyGame() => OnDestroyGame?.Invoke(this);
    }

    public class SynzzaGameDependent {
        protected SynzzaGame _game;

        public SynzzaGameDependent(SynzzaGame game) {
            _game = game;
            _game.OnDestroyGame += OnDestroyGame;
        }

        protected virtual void OnDestroyGame(SynzzaGame destroyedGame) {
            if (_game == destroyedGame) {
                _game = null;
            }
        }
    }

    public sealed class SingletonSynzzaGame : SynzzaGame {
        private static SingletonSynzzaGame _current = null;

        public static void CreateGame(byte playerFactionID) {
            if (_current != null) {
                throw new InvalidOperationException($"Cannot overwrite existing {typeof(SingletonSynzzaGame).Name} instance!");
            }

            _current = new(playerFactionID);
        }

        public static void DestroyGame() {
            if (_current == null) {
                throw new InvalidOperationException($"No existing {typeof(SingletonSynzzaGame).Name} instance to destroy!");
            }

            _current.InvokeOnDestroyGame();
            _current = null;
        }

        public static SingletonSynzzaGame Current => _current;

        public SingletonGameInitializationStage InitializationStage { get; private set; }

        private SingletonGameInitializationStage PeniultimateInitializationStage => SingletonGameInitializationStage.SkillAttackRangeProfileRegistrationCompleted;

        public override BattlerFactionRegistry BattlerFactions { get; }
        public override SpawnLocationProfileRegistry SpawnLocations { get; }
        public override SkillRegistry Skills { get; }
        public override SkillAttackRangeProfileRegistry SkillAttackRanges { get; }
        public override BattleCoordinator Battles { get; }
        public override byte PlayerFactionID { get; }

        private IWorld _currentWorld;

        private SingletonSynzzaGame(byte playerFactionID) {
            BattlerFactions = new();
            SpawnLocations = new();
            Skills = new();
            SkillAttackRanges = new();
            Battles = new();
            _currentWorld = null;
            InitializationStage = SingletonGameInitializationStage.Created;
            PlayerFactionID = playerFactionID;
        }

        private bool IsCompletionInitializationStage(SingletonGameInitializationStage stage) => stage switch {
            SingletonGameInitializationStage.Created
                or SingletonGameInitializationStage.BattlerFactionRegistrationCompleted
                or SingletonGameInitializationStage.SpawnLocationProfileRegistrationCompleted
                or SingletonGameInitializationStage.SkillRegistrationCompleted
                or SingletonGameInitializationStage.SkillAttackRangeProfileRegistrationCompleted
                or SingletonGameInitializationStage.Done => true,
            _ => false,
        };

        private SingletonGameInitializationStage GetCompletionInitializationStage(SingletonGameInitializationStage stage) => stage switch {
            SingletonGameInitializationStage.BattlerFactionRegistration => SingletonGameInitializationStage.BattlerFactionRegistrationCompleted,
            SingletonGameInitializationStage.SpawnLocationProfileRegistration => SingletonGameInitializationStage.SpawnLocationProfileRegistrationCompleted,
            SingletonGameInitializationStage.SkillRegistration => SingletonGameInitializationStage.SkillRegistrationCompleted,
            SingletonGameInitializationStage.SkillAttackRangeProfileRegistration => SingletonGameInitializationStage.SkillAttackRangeProfileRegistrationCompleted,
            _ => throw new ArgumentException($"{stage} has no completion stage!")
        };

        public void BeginInitializationStage(SingletonGameInitializationStage stage) {
            if (IsCompletionInitializationStage(stage)) {
                throw new ArgumentException($"{typeof(SingletonGameInitializationStage).Name}.{stage} is not valid for this operation!");
            }

            if (InitializationStage >= stage) {
                throw new InvalidOperationException($"This {GetType().Name} instance has moved past or is on the \"{stage}\" {typeof(SingletonGameInitializationStage).Name}.");
            }

            InitializationStage = stage;
        }

        public void CompleteInitializationStage(SingletonGameInitializationStage completionStage) {
            if (completionStage != GetCompletionInitializationStage(InitializationStage)) {
                throw new InvalidOperationException($"{typeof(SingletonGameInitializationStage).Name}.{completionStage} is not valid for this operation!");
            }

            InitializationStage = completionStage;
        }

        public void SetInitializationDone() {
            if (InitializationStage != PeniultimateInitializationStage) {
                throw new InvalidOperationException($"This {GetType().Name} instance has not yet completed initialization!");
            }

            InitializationStage = SingletonGameInitializationStage.Done;
        }

        public override IWorld GetCurrentWorld() => _currentWorld;
        public override void SetCurrentWorld(IWorld world) {
            if (_currentWorld != null) {
                throw new InvalidOperationException($"Cannot overwrite existing World instance!");
            }

            _currentWorld = world;
            _currentWorld.OnExit += ResetCurrentWorld;
        }

        private void ResetCurrentWorld() => _currentWorld = null;
    }
}
