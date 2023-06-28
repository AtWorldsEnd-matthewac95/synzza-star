using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    [CreateAssetMenu(fileName = "NewSkillEffectDurationProfile", menuName = "Scrib/SkillEffectDurationProfile")]
    public class SkillEffectDurationProfileScrib : ScriptableObject {
        [SerializeField] private bool _isIndefinite;
        [SerializeField] [Min(0f)] private float _durationSeconds;

        public bool IsIndefinite => _isIndefinite;
        public float DurationSeconds => _durationSeconds;
    }
}
