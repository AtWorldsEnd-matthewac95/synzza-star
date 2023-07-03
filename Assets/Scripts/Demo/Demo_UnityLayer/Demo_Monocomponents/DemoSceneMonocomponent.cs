using UnityEngine;
using AWE.Synzza.UnityLayer;

namespace AWE.Synzza.Demo.UnityLayer {
    public class DemoSceneMonocomponent : MonoBehaviour, IScene {
        [SerializeField] private BattlerMonocomponent _player;

        private void Start() {
            Debug.Assert(_player != null, $"{GetType().Name} {gameObject.name} does not have a reference to the player transform!");
        }

        public ISceneObject FindRandomPlayer() => new UnitySceneObject(_player.transform);

        public bool IsSceneObjectPlayer(ISceneObject obj) => obj is UnitySceneObject transform && transform.transform == _player.transform;
    }
}
