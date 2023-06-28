using AWE.Synzza.UnityLayer.Monocomponents.UI;
using AWE.Synzza.UnityLayer.Scrib;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Monocomponents {
    public class EnemyBattlerMonocomponent : MonoBehaviour, IBattlerMonocomponent {
        [SerializeField] private BattlerScrib _battler;
        [SerializeField] private BattlerStatusUI _statusText;

        public Battler EnemyBattler { get; private set; }
        public Battler Battler => EnemyBattler;
        public string DisplayName => EnemyBattler.DisplayName;

        private void Awake() {
            EnemyBattler = _battler.ToBattler(BattlerTeam.Enemies);
            if (_statusText != null) { _statusText.BattlerMono = this; }
        }
    }
}
