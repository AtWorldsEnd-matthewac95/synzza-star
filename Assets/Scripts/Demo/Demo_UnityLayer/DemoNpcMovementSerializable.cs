using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using AWE.Synzza.UnityLayer;
using AWE.Synzza.Demo.UnityLayer.Monocomponents;

namespace AWE.Synzza.Demo.UnityLayer {
    [Serializable]
    public class DemoNpcMovementSerializable {
        [Tooltip("SceneMonocomponent instance for finding player targets.")]
        [SerializeField] private DemoSceneMonocomponent _sceneMonocomponent;
        [Tooltip("List of static targets which this agent will move towards. Note that if these Transforms are not static (i.e. can move), when the agent chooses the target it will simply move to wherever the object was at that moment.")]
        [SerializeField] private Transform[] _positionalTargets;
        [Tooltip("The agent will only select from the first X positional targets, where X is this number. After selecting a target, it will be swapped in the list with an element past the first X, thereby making that target unselectable for a time.")]
        [SerializeField] private int _validPositionalTargetCount;
        [Tooltip("% chance the AI will select the player instead of a static positional target.")]
        [SerializeField] [Range(0f, 1f)] private float _targetPlayerChance;

        private IScene _scene = null;

        public DemoNpcMovementSerializable(IScene scene, int validPositionalTargetCount, float targetPlayerChance, IEnumerable<Transform> positionalTargets) {
            _scene = scene;
            _validPositionalTargetCount = validPositionalTargetCount;
            _targetPlayerChance = targetPlayerChance;
            _positionalTargets = positionalTargets.ToArray();
        }

        public IScene SceneMonocomponent => _sceneMonocomponent;
        public ReadOnlySpan<Transform> PositionalTargets => new(_positionalTargets);
        public int ValidPositionalTargetCount => _validPositionalTargetCount;
        public float TargetPlayerChance => _targetPlayerChance;

        public DemoNpcMovement ToDemoNpcMovement() => new(
            _scene ?? _sceneMonocomponent,
            _validPositionalTargetCount,
            _targetPlayerChance,
            _positionalTargets.Select<Transform, ISceneObject>(t => new UnitySceneObject(t, isMobile: false))
        );
    }
}
