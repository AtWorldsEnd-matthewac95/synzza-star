using System.Collections;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    /*
     * This script is configured to execute before the default time. In the Unity editor, go to Edit->Project Settings->Script Execution Order for more details.
     */
    public sealed class BattlerFactionRegistryMonocomponent : MonoBehaviour {
        [SerializeField] private BattlerFactionScrib[] _factionsToRegister;

        private BattlerFactionRegistry _registry = null;

        private void Awake() {
            var game = SynzzaGame.Current;
            game.BeginInitializationStage(GameInitializationStage.BattlerFactionRegistration);

            _registry = game.BattlerFactions;
            foreach (var scrib in _factionsToRegister) {
                _registry.RegisterFaction(scrib.ToBattlerFaction());
            }

            game.CompleteInitializationStage(GameInitializationStage.BattlerFactionRegistrationCompleted);
        }
    }
}
