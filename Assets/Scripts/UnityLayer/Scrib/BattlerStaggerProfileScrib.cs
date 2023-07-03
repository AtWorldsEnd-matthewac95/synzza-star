using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [CreateAssetMenu(fileName = "NewBattlerStaggerProfile", menuName = "Scrib/BattlerStaggerProfile")]
    public class BattlerStaggerProfileScrib : ScriptableObject {
        [SerializeField] private float _durationSeconds;

        public float DurationSeconds => _durationSeconds;

        public BattlerStaggerProfile ToBattlerStaggerProfile() => new BattlerStaggerProfile(_durationSeconds);
    }
}
