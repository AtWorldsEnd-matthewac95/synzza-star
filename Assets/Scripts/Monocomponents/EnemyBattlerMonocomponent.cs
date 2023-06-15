using UnityEngine;

namespace AWE.Synzza.Monocomponents {
    public class EnemyBattlerMonocomponent : MonoBehaviour, IBattlerMonocomponent {
        [SerializeField] private string _displayName;

        public Battler EnemyBattler { get; private set; }
        public string DisplayName => EnemyBattler?.DisplayName;

        private void Awake() {
            EnemyBattler = new Battler(BattlerTeam.Enemies, _displayName);
        }
    }
}
