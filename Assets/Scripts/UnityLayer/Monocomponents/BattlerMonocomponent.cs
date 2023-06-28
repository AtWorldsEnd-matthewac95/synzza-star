using UnityEngine;

namespace AWE.Synzza.UnityLayer.Monocomponents {
    public interface IBattlerMonocomponent {
        string DisplayName { get; }
        Battler Battler { get; }
        Transform transform { get; }
    }
}
