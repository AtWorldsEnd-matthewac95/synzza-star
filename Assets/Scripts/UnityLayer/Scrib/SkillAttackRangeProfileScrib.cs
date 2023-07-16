using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [CreateAssetMenu(fileName = "NewSkillAttackRangeProfile", menuName = "Scrib/SkillAttackRangeProfile")]
    public class SkillAttackRangeProfileScrib : ScriptableObject {
        [SerializeField] private bool _isUsingInnateAttackRange;
        [SerializeField] private byte _id;
        [SerializeField] [Min(0f)] private float _min;
        [SerializeField] [Min(0f)] private float _max;

        public bool IsUsingInnateAttackRange => _isUsingInnateAttackRange;
        public byte ID => _id;
        public FloatRange AttackRange => new(_min, Mathf.Max(_max, _min));

        public SkillAttackRangeProfile ToSkillAttackRangeProfile() => new(_id, AttackRange);
    }
}
