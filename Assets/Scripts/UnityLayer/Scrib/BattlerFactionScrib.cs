using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public enum BattlerFactionID : byte {
        None,
        Player,
        Enemies
    }

    [CreateAssetMenu(fileName = "NewBattlerFaction", menuName = "Scrib/BattlerFaction")]
    public sealed class BattlerFactionScrib : ScriptableObject {
        [SerializeField] private BattlerFactionID _id;
        [SerializeField] private string _displayName;
        [SerializeField] private bool _isFriendly;
        [SerializeField] private bool _isHostile;

        public BattlerFactionID ID => _id;
        public byte ByteID => (byte)_id;
        public string Displayname => _displayName;
        public bool IsFriendly => _isFriendly;
        public bool IsHostile => _isHostile;

        public BattlerFaction ToBattlerFaction() => new(ByteID, _displayName, _isFriendly, _isHostile);
    }
}
