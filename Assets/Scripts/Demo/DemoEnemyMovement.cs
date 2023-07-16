using System;
using System.Collections.Generic;
using System.Linq;

namespace AWE.Synzza.Demo {
    public class DemoEnemyMovement : INpcMovement {
        public const int MIN_VALID_POSITIONAL_TARGET_COUNT = 2;

        private readonly IWorldObject[] _positionalTargets;
        private int _validPositionalTargetCount;
        private int _positionalTargetSwapIndex = 0;

        public ReadOnlySpan<IWorldObject> PositionalTargets => new(_positionalTargets);
        public float TargetPlayerChance { get; set; }
        public int ValidPositionalTargetCount {
            get => _validPositionalTargetCount;
            set => _validPositionalTargetCount = Math.Min(Math.Max(value, MIN_VALID_POSITIONAL_TARGET_COUNT), _positionalTargets.Length);
        }
        public float Speed { get; set; }

        public DemoEnemyMovement(int validPositionalTargetCount, float targetPlayerChance, IEnumerable<IWorldObject> positionalTargets, float speed) {
            _positionalTargets = positionalTargets.ToArray();
            ValidPositionalTargetCount = validPositionalTargetCount;
            TargetPlayerChance = targetPlayerChance;
            Speed = speed;
        }

        public IWorldObject PickNewMovementTarget(IWorldObject previousTarget) {
            if ((previousTarget is not IBattlerWorldObject previousTargetBattler || previousTargetBattler.Battler.FactionID != SingletonSynzzaGame.Current.PlayerFactionID)
                && (GameRandom.NextFloat() < TargetPlayerChance)
            ) {
                return SingletonSynzzaGame.Current.GetCurrentWorld().FindClosestBattler(default, SingletonSynzzaGame.Current.PlayerFactionID);
            }

            var index = GameRandom.Range(0, _validPositionalTargetCount);
            var target = _positionalTargets[index];

            var swapIndex = _validPositionalTargetCount + _positionalTargetSwapIndex;
            _positionalTargetSwapIndex = (_positionalTargetSwapIndex + 1) % (_positionalTargets.Length - _validPositionalTargetCount);

            _positionalTargets[index] = _positionalTargets[swapIndex];
            _positionalTargets[swapIndex] = target;

            return target;
        }
    }
}
