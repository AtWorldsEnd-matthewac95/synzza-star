using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    /*
     * This script is configured to execute before the default time. In the Unity editor, go to Edit->Project Settings->Script Execution Order for more details.
     */
    public sealed class SpawnLocationProfileRegistryMonocomponent : MonoBehaviour {
        [SerializeField] private SpawnLocationProfileScrib[] _spawnLocationProfilesToRegister;

        private SpawnLocationProfileRegistry _registry = null;

        private void Awake() {
            var game = SingletonSynzzaGame.Current;
            game.BeginInitializationStage(SingletonGameInitializationStage.SpawnLocationProfileRegistration);

            _registry = game.SpawnLocations;
            for (int i = 0; i < _spawnLocationProfilesToRegister.Length; ++i) {
                _registry.Register(_spawnLocationProfilesToRegister[i].ToSpawnLocationProfile());
            }

            game.CompleteInitializationStage(SingletonGameInitializationStage.SpawnLocationProfileRegistrationCompleted);
        }
    }
}
