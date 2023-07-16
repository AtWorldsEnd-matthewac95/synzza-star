using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    /*
     * This script is configured to execute before the default time. In the Unity editor, go to Edit->Project Settings->Script Execution Order for more details.
     */
    public sealed class SkillAttackRangeProfileRegistryMonocomponent : MonoBehaviour {
        [SerializeField] private SkillAttackRangeProfileScrib[] _profilesToRegister;

        private SkillAttackRangeProfileRegistry _registry = null;

        private void Awake() {
            var game = SingletonSynzzaGame.Current;
            game.BeginInitializationStage(SingletonGameInitializationStage.SkillAttackRangeProfileRegistration);

            _registry = game.SkillAttackRanges;
            foreach (var scrib in _profilesToRegister) {
                if (!scrib.IsUsingInnateAttackRange) {
                    _registry.Register(scrib.ToSkillAttackRangeProfile());
                }
            }

            game.CompleteInitializationStage(SingletonGameInitializationStage.SkillAttackRangeProfileRegistrationCompleted);
            game.SetInitializationDone();
        }
    }
}
