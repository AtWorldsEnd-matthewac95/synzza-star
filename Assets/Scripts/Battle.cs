using System;

namespace AWE.Synzza {
    public sealed class Battle {
        public bool IsEnded { get; private set; }
        public IWorld World { get; }

        private readonly BattlerWorldCatalog<IBattlerWorldObject> _battlers = new();

        public event BattleEndDelegate OnBattleEnd;
        public event BattleDelayedStartDelegate OnDelayedStart;

        public Battle(IWorld world, params IBattlerWorldObject[] initialBattlers) {
            World = world;
            World.OnExit += End;

            foreach (var battler in initialBattlers) {
                _battlers.AddBattler(battler);
            }

            if (_battlers.FindFactionCount() <= 1) {
                throw new ArgumentException($"Cannot create an instance of {GetType().Name} without two or more distinct {typeof(BattlerFaction).Name} values!");
            }

            IsEnded = false;
        }

        private void End() {
            IsEnded = true;
            OnBattleEnd?.Invoke(this);
        }
    }
}
