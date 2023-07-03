using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    /*
     * This script is configured to execute before the default time. In the Unity editor, go to Edit->Project Settings->Script Execution Order for more details.
     */
    public class SynzzaGameMonocomponent : MonoBehaviour {
        private void Awake() {
            DontDestroyOnLoad(this);
            QuaternionMath.ProvideCalculator(new UnityQuaternionMath());
            SynzzaGame.CreateGame();
        }

        private void OnDestroy() {
            SynzzaGame.DestroyGame();
            QuaternionMath.ReleaseCalculator();
        }
    }
}
