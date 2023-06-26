using AWE.Synzza.UnityLayer.Scrib;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Monocomponents {
    public class PlayerBattlerMonocomponent : MonoBehaviour, IBattlerMonocomponent {
        [SerializeField] private BattlerScrib _battler;

        public Battler PlayerBattler { get; private set; }
        public string DisplayName => PlayerBattler.DisplayName;

        private void Awake() {
            PlayerBattler = _battler.ToBattler(BattlerTeam.Players);
        }
    }
}
