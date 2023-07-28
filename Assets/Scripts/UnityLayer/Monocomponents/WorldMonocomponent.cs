using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    /*
     * This script is configured to execute before the default time. In the Unity editor, go to Edit->Project Settings->Script Execution Order for more details.
     */
    public class WorldMonocomponent : MonoBehaviour {
        [SerializeField] private BattlerNavAgentMonocomponent _player;

        private UnityWorld _world = null;

        private void Start() {
            Debug.Assert(_player != null, $"{GetType().Name} {gameObject.name} does not have a reference to the player transform!");
            _world = new(SingletonSynzzaGame.Current, _player);
        }

        private void OnDestroy() {
            _world.ForceExit();
        }
    }
}
