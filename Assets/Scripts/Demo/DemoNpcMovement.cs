using System;
using System.Collections.Generic;
using System.Linq;

namespace AWE.Synzza.Demo {
    public class DemoNpcMovement : INpcMovement {
        public const int MIN_VALID_POSITIONAL_TARGET_COUNT = 2;

        private readonly ISceneObject[] _positionalTargets;
        private int _validPositionalTargetCount;
        private int _positionalTargetSwapIndex = 0;

        public IScene Scene { get; }
        public ReadOnlySpan<ISceneObject> PositionalTargets => new(_positionalTargets);
        public float TargetPlayerChance { get; set; }
        public int ValidPositionalTargetCount {
            get => _validPositionalTargetCount;
            set => _validPositionalTargetCount = Math.Min(Math.Max(value, MIN_VALID_POSITIONAL_TARGET_COUNT), _positionalTargets.Length);
        }

        public DemoNpcMovement(IScene scene, int validPositionalTargetCount, float targetPlayerChance, IEnumerable<ISceneObject> positionalTargets) {
            Scene = scene;
            _positionalTargets = positionalTargets.ToArray();
            ValidPositionalTargetCount = validPositionalTargetCount;
            TargetPlayerChance = targetPlayerChance;
        }

        public ISceneObject PickNewMovementTarget(ISceneObject previousTarget) {
            if (!Scene.IsSceneObjectPlayer(previousTarget) && (GameRandom.NextFloat() < TargetPlayerChance)) {
                return Scene.FindRandomPlayer();
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
