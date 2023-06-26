using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    [CreateAssetMenu(fileName = "NewSkillDisplayProfile", menuName = "Scrib/SkillDisplayProfile")]
    public class SkillDisplayProfileScrib : ScriptableObject {
        [SerializeField] [Min(0f)] private float _durationSeconds;

        public float DurationSeconds => _durationSeconds;
    }
}
