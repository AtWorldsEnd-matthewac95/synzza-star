﻿using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    [CreateAssetMenu(fileName = "NewSkillEffectDurationProfile", menuName = "Scrib/SkillEffectDurationProfile")]
    public class SkillEffectDurationProfileScrib : ScriptableObject {
        [SerializeField] private bool _isIndefinite;
        [SerializeField] [Min(0f)] private float _durationSeconds;

        public (bool isIndefinite, float durationSeconds) ToTuple() => (_isIndefinite, _durationSeconds);
    }
}
