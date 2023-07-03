using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    /*
     * This script is configured to execute before the default time. In the Unity editor, go to Edit->Project Settings->Script Execution Order for more details.
     */
    public sealed class SpawnProfileRegistryMonocomponent : MonoBehaviour {
        [SerializeField] private SpawnProfileScrib[] _spawnProfilesToRegister;

        private SpawnProfileRegistry _registry = null;

        private void Awake() {
            var game = SynzzaGame.Current;
            game.BeginInitializationStage(GameInitializationStage.SpawnProfileRegistration);

            _registry = game.SpawnProfiles;
            for (int i = 0; i < _spawnProfilesToRegister.Length; ++i) {
                _registry.RegisterSpawnProfile(_spawnProfilesToRegister[i].ToSpawnProfile((byte)i));
            }

            game.CompleteInitializationStage(GameInitializationStage.SpawnProfileRegistrationCompleted);
        }
    }
}
