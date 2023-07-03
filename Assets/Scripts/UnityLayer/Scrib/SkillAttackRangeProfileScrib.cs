using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [CreateAssetMenu(fileName = "NewSkillAttackRangeProfile", menuName = "Scrib/SkillAttackRangeProfile")]
    public class SkillAttackRangeProfileScrib : ScriptableObject {
        [SerializeField] private bool _isUsingInnateAttackRange;
        [SerializeField] [Min(0f)] private float _min;
        [SerializeField] [Min(0f)] private float _max;

        public bool IsUsingInnateAttackRange => _isUsingInnateAttackRange;
        public FloatRange AttackRange => new(_min, Mathf.Max(_max, _min));
    }
}
