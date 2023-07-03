using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    /*
     * This script is configured to execute before the default time. In the Unity editor, go to Edit->Project Settings->Script Execution Order for more details.
     */
    public class BattlerMonocomponent : MonoBehaviour {
        [SerializeField] protected BattlerScrib _battler;
        [SerializeField] protected BattlerFactionScrib _faction;
        [SerializeField] protected BattlerStatusUI _statusText;

        public Battler Battler { get; private set; }

        protected virtual void Awake() {
            Battler = _battler.ToBattler();
            Battler.SetFaction(_faction.ToBattlerFaction());

            if (_statusText != null) {
                _statusText.BattlerMono = this;
            }
        }
    }
}
