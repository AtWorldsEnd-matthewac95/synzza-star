using UnityEngine;

namespace AWE.Synzza.Monocomponents {
    public class PlayerBattlerMonocomponent : MonoBehaviour, IBattlerMonocomponent {
        [SerializeField] private string _displayName;

        public Battler PlayerBattler { get; private set; }
        public string DisplayName => PlayerBattler?.DisplayName;

        private void Awake() {
            PlayerBattler = new Battler(BattlerTeam.Players, _displayName);
        }
    }
}
