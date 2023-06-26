using UnityEngine;

using AWE.Synzza.UnityLayer;
using AWE.Synzza.UnityLayer.Monocomponents;

namespace AWE.Synzza.Demo.UnityLayer.Monocomponents {
    public class DemoSceneMonocomponent : MonoBehaviour, IScene {
        [SerializeField] private PlayerControllerMonocomponent _player;

        private void Start() {
            Debug.Assert(_player != null, $"{GetType().Name} {gameObject.name} does not have a reference to the player transform!");
        }

        public ISceneObject FindRandomPlayer() => new UnitySceneObject(_player.transform);

        public bool IsSceneObjectPlayer(ISceneObject obj) => obj is UnitySceneObject transform && transform.transform == _player.transform;
    }
}
