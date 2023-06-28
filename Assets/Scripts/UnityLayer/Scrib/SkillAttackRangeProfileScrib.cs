using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    [CreateAssetMenu(fileName = "NewSkillAttackRangeProfile", menuName = "Scrib/SkillAttackRangeProfile")]
    public class SkillAttackRangeProfileScrib : ScriptableObject {
        [SerializeField] private bool _isUsingDefaultAttackRange;
        [SerializeField] [Min(0f)] private float _min;
        [SerializeField] [Min(0f)] private float _max;

        public bool IsUsingDefaultAttackRange => _isUsingDefaultAttackRange;
        public FloatRange AttackRange => new(_min, Mathf.Max(_max, _min));
    }
}
