using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [CreateAssetMenu(fileName = "NewSkillDisplayProfile", menuName = "Scrib/SkillDisplayProfile")]
    public class SkillDisplayProfileScrib : ScriptableObject {
        [SerializeField] [Min(0f)] private float _durationSeconds;
    }
}
