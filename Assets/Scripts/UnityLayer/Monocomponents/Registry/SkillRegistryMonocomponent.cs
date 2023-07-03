using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    /*
     * This script is configured to execute before the default time. In the Unity editor, go to Edit->Project Settings->Script Execution Order for more details.
     */
    public sealed class SkillRegistryMonocomponent : MonoBehaviour {
        [SerializeField] private SkillScrib[] _skillsToRegister;

        private SkillRegistry _registry = null;

        private void Awake() {
            var game = SynzzaGame.Current;
            game.BeginInitializationStage(GameInitializationStage.SkillRegistration);

            _registry = game.Skills;
            for (int i = 0; i < _skillsToRegister.Length; ++i) {
                _registry.RegisterSkill(_skillsToRegister[i].ToSkill());
            }

            game.CompleteInitializationStage(GameInitializationStage.SkillRegistrationCompleted);
            game.SetInitializationDone();
        }
    }
}
