using AWE.Synzza.UnityLayer.Scrib;
using UnityEngine;

namespace AWE.Synzza.UnityLayer.Monocomponents {
    public class EnemyBattlerMonocomponent : MonoBehaviour, IBattlerMonocomponent {
        [SerializeField] private BattlerScrib _battler;

        public Battler EnemyBattler { get; private set; }
        public string DisplayName => EnemyBattler.DisplayName;

        private void Awake() {
            EnemyBattler = _battler.ToBattler(BattlerTeam.Enemies);
        }
    }
}
