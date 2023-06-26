using UnityEngine;

namespace AWE.Synzza.UnityLayer.Scrib {
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Scrib/Skill")]
    public class SkillScrib : ScriptableObject {
        [SerializeField] private string _displayName;
        [SerializeField] private bool _isUsingDefaultCooldownTurns;
        [SerializeField] [Min(1)] private int _cooldownTurns = 1;
        [SerializeField] [Min(0f)] private float _windUpDurationSeconds;
        [SerializeField] [Min(0f)] private float _windUpVarianceSeconds;
        [SerializeField] private bool _isEffectIndefinite;
        [SerializeField] [Min(0f)] private float _effectDurationSeconds;
        [SerializeField] [Min(0f)] private float _windDownDurationSeconds;
        [SerializeField] private bool _isInterruptible;
        [SerializeField] private bool _isUsingDefaultAttackRange;
        [SerializeField] [Min(0f)] private float _minAttackRange;
        [SerializeField] [Min(0f)] private float _maxAttackRange;
        [SerializeField] private GameObject _projectile;
        [SerializeField] private SpawnProfileScrib _spawnProfile;
        [SerializeField] private SkillDisplayProfileScrib _displayProfile;

        public string DisplayName => _displayName;
        public bool IsUsingDefaultCooldownTurns => _isUsingDefaultCooldownTurns;
        public int CooldownTurns => _cooldownTurns;
        public float WindUpDurationSeconds => _windUpDurationSeconds;
        public float WindUpVarianceSeconds => _windUpVarianceSeconds;
        public bool IsEffectIndefinite => _isEffectIndefinite;
        public float EffectDurationSeconds => _effectDurationSeconds;
        public float WindDownDurationSeconds => _windDownDurationSeconds;
        public float TotalDurationMinSeconds => _windUpDurationSeconds + (_isEffectIndefinite ? 0f : _effectDurationSeconds) + _windDownDurationSeconds;
        public float TotalDurationMaxSeconds => TotalDurationMinSeconds + _windUpVarianceSeconds;
        public bool IsInterruptible => _isInterruptible;
        public bool IsUsingDefaultAttackRange => _isUsingDefaultAttackRange;
        public FloatRange AttackRange => new(_minAttackRange, Mathf.Max(_maxAttackRange, _minAttackRange));
        public SpawnProfileScrib SpawnProfile => _spawnProfile;
        public SkillDisplayProfileScrib DisplayProfile => _displayProfile;
    }
}
