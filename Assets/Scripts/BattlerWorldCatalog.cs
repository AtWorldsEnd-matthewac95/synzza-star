using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

namespace AWE.Synzza {
    public interface IBattlerWorldCatalog {
        event AddBattlerToWorldCatalogDelegate OnAddBattler;
        event RemoveBattlerFromWorldCatalogDelegate OnRemoveBattler;

        void ForEachBattler(ForEachBattlerWorldObjectEarlyExitDelegate forEachBattler);

        void ForEachBattlerWithFactionID(byte factionID, ForEachBattlerWorldObjectEarlyExitDelegate forEachBattler);

        bool AddBattler(in IBattlerWorldObject battler, WorldObjectEqualityDelegate equalityDelegateOverride = null);

        bool RemoveBattler(in IBattlerWorldObject battler, WorldObjectEqualityDelegate equalityDelegateOverride = null);

        bool TryFindBattler(out IBattlerWorldObject foundBattler, ForEachBattlerWorldObjectEarlyExitDelegate criteria);
        bool TryFindBattler(out IBattlerWorldObject foundBattler, byte factionID, ForEachBattlerWorldObjectEarlyExitDelegate criteria);

        // TODO - Make a version of these functions that excludes all battlers of a certain faction ID. Currently, the user can only specify one ID to find, or find amonst all battlers.
        IBattlerWorldObject FindClosestBattler(in IBattlerWorldObject toMe, byte withFactionID = BattlerFaction.ID_NONE, WorldObjectEqualityDelegate equalityDelegateOverride = null);
        IBattlerWorldObject FindClosestBattler(
            in float3 position,
            in IBattlerWorldObject ignore = default,
            byte withFactionID = BattlerFaction.ID_NONE,
            WorldObjectEqualityDelegate equalityDelegateOverride = null
        );

        int FindFactionCount();

        int FindBattlerCountWithFaction(byte factionID);
    }

    public interface IBattlerWorldCatalog<TBattlerWorldObject> : IBattlerWorldCatalog where TBattlerWorldObject : class, IBattlerWorldObject {
        bool AddBattler(in TBattlerWorldObject battler, WorldObjectEqualityDelegate equalityDelegateOverride = null);

        bool RemoveBattler(in TBattlerWorldObject battler, WorldObjectEqualityDelegate equalityDelegateOverride = null);

        bool TryFindBattler(out TBattlerWorldObject foundBattler, ForEachBattlerWorldObjectEarlyExitDelegate criteria);
        bool TryFindBattler(out TBattlerWorldObject foundBattler, byte factionID, ForEachBattlerWorldObjectEarlyExitDelegate criteria);

        // TODO - Make a version of these functions that excludes all battlers of a certain faction ID. Currently, the user can only specify one ID to find, or find amonst all battlers.
        new TBattlerWorldObject FindClosestBattler(in IBattlerWorldObject toMe, byte withFactionID = BattlerFaction.ID_NONE, WorldObjectEqualityDelegate equalityDelegateOverride = null);
        new TBattlerWorldObject FindClosestBattler(
            in float3 position,
            in IBattlerWorldObject ignore = default,
            byte withFactionID = BattlerFaction.ID_NONE,
            WorldObjectEqualityDelegate equalityDelegateOverride = null
        );
    }

    public sealed class BattlerWorldCatalog<TBattlerWorldObject> : IBattlerWorldCatalog<TBattlerWorldObject> where TBattlerWorldObject : class, IBattlerWorldObject {
        private readonly LinkedList<TBattlerWorldObject> _allBattlers;
        private readonly Dictionary<byte, LinkedList<TBattlerWorldObject>> _factionBattlers;

        public event AddBattlerToWorldCatalogDelegate OnAddBattler;
        public event RemoveBattlerFromWorldCatalogDelegate OnRemoveBattler;

        public BattlerWorldCatalog() {
            _allBattlers = new();
            _factionBattlers = new();
        }

        public void ForEachBattler(ForEachBattlerWorldObjectEarlyExitDelegate forEachBattler) {
            int index = 0;
            foreach(var battler in _allBattlers) {
                if (forEachBattler(battler, index++)) {
                    break;
                }
            }
        }

        public void ForEachBattlerWithFactionID(byte factionID, ForEachBattlerWorldObjectEarlyExitDelegate forEachBattler) {
            if (_factionBattlers.ContainsKey(factionID)) {
                int index = 0;
                foreach(var battler in _factionBattlers[factionID]) {
                    if (forEachBattler(battler, index++)) {
                        break;
                    }
                }
            }
        }

        private bool DefaultWorldObjectEquality(in IMutableWorldObject lhs, in IMutableWorldObject rhs) => lhs.Equals(rhs);

        public bool AddBattler(in TBattlerWorldObject battler, WorldObjectEqualityDelegate equalityDelegateOverride = null) {
            if (battler == null) {
                return false;
            }

            var factionID = battler.Battler.FactionID;
            if (factionID == BattlerFaction.ID_NONE) {
                return false;
            }

            if (!_factionBattlers.ContainsKey(factionID)) {
                _factionBattlers.Add(factionID, new());
            }

            _factionBattlers[factionID].AddLast(battler);
            _allBattlers.AddLast(battler);

            equalityDelegateOverride ??= DefaultWorldObjectEquality;
            battler.OnPreDestroy += (in IMutableWorldObject battler) => RemoveBattler(battler as TBattlerWorldObject, equalityDelegateOverride);

            OnAddBattler?.Invoke(battler);
            return true;
        }
        bool IBattlerWorldCatalog.AddBattler(in IBattlerWorldObject battler, WorldObjectEqualityDelegate equalityDelegateOverride)
            => battler is TBattlerWorldObject tbattler && AddBattler(tbattler, equalityDelegateOverride);

        public bool RemoveBattler(in TBattlerWorldObject battler, WorldObjectEqualityDelegate equalityDelegateOverride = null) {
            if (battler == null) {
                return false;
            }

            equalityDelegateOverride ??= DefaultWorldObjectEquality;

            if (!RemoveBattlerFromList(battler, equalityDelegateOverride, _allBattlers)) {
                return false;
            }

            var factionID = battler.Battler.FactionID;
            if (!_factionBattlers.ContainsKey(factionID)) {
                throw new Exception($"Battler {battler.Battler.DisplayName} was removed from the all battlers list but not in a faction list!");
            }

            if (!RemoveBattlerFromList(battler, equalityDelegateOverride, _allBattlers)) {
                throw new Exception($"Battler {battler.Battler.DisplayName} was removed from the all battlers list but could not be removed from a faction list!");
            }

            OnRemoveBattler?.Invoke(battler);
            return true;
        }
        bool IBattlerWorldCatalog.RemoveBattler(in IBattlerWorldObject battler, WorldObjectEqualityDelegate equalityDelegateOverride)
            => battler is TBattlerWorldObject tbattler && RemoveBattler(tbattler, equalityDelegateOverride);

        private bool RemoveBattlerFromList(in TBattlerWorldObject battler, WorldObjectEqualityDelegate equality, LinkedList<TBattlerWorldObject> list) {
            bool isRemoved = false;

            LinkedListNode<TBattlerWorldObject> nodeToRemove = list.Last;
            while (nodeToRemove != null) {
                if (equality(battler, nodeToRemove.Value)) {
                    list.Remove(nodeToRemove);
                    isRemoved = true;
                    break;
                }
                nodeToRemove = nodeToRemove.Previous;
            }

            return isRemoved;
        }

        public bool TryFindBattler(out TBattlerWorldObject foundBattler, ForEachBattlerWorldObjectEarlyExitDelegate criteria) => TryFindBattler(out foundBattler, BattlerFaction.ID_NONE, criteria);
        public bool TryFindBattler(out TBattlerWorldObject foundBattler, byte factionID, ForEachBattlerWorldObjectEarlyExitDelegate criteria) {
            foundBattler = default;
            var list = _factionBattlers.ContainsKey(factionID) ? _factionBattlers[factionID] : _allBattlers;

            int index = 0;
            foreach (var battler in list) {
                if (criteria(battler, index++)) {
                    foundBattler = battler;
                    return true;
                }
            }

            return false;
        }
        bool IBattlerWorldCatalog.TryFindBattler(out IBattlerWorldObject foundBattler, ForEachBattlerWorldObjectEarlyExitDelegate criteria) {
            bool success = TryFindBattler(out var foundTBattler, criteria);
            foundBattler = success ? foundTBattler : default;
            return success;
        }
        bool IBattlerWorldCatalog.TryFindBattler(out IBattlerWorldObject foundBattler, byte factionID, ForEachBattlerWorldObjectEarlyExitDelegate criteria) {
            bool success = TryFindBattler(out var foundTBattler, factionID, criteria);
            foundBattler = success ? foundTBattler : default;
            return success;
        }

        // TODO - Make a version of these functions that excludes all battlers of a certain faction ID. Currently, the user can only specify one ID to find, or find amonst all battlers.
        public TBattlerWorldObject FindClosestBattler(in IBattlerWorldObject toMe, byte withFactionID = BattlerFaction.ID_NONE, WorldObjectEqualityDelegate equalityDelegateOverride = null)
            => FindClosestBattler(toMe.WorldPosition, toMe, withFactionID, equalityDelegateOverride);
        public TBattlerWorldObject FindClosestBattler(
            in float3 position,
            in IBattlerWorldObject ignore = default,
            byte withFactionID = BattlerFaction.ID_NONE,
            WorldObjectEqualityDelegate equalityDelegateOverride = null
        ) {
            equalityDelegateOverride ??= DefaultWorldObjectEquality;
            var list = _factionBattlers.ContainsKey(withFactionID) ? _factionBattlers[withFactionID] : _allBattlers;

            var closestValue = float.PositiveInfinity;
            TBattlerWorldObject closestBattler = default;

            foreach (var battler in list) {
                var magnitudeSquared = battler == null ? float.PositiveInfinity : (battler.WorldPosition - position).MagnitudeSquared;
                if (!equalityDelegateOverride(ignore, battler) && magnitudeSquared < closestValue) {
                    closestValue = magnitudeSquared;
                    closestBattler = battler;
                }
            }

            return closestBattler;
        }
        IBattlerWorldObject IBattlerWorldCatalog.FindClosestBattler(in IBattlerWorldObject toMe, byte withFactionID, WorldObjectEqualityDelegate equalityDelegateOverride)
            => FindClosestBattler(toMe, withFactionID, equalityDelegateOverride);
        IBattlerWorldObject IBattlerWorldCatalog.FindClosestBattler(in float3 position, in IBattlerWorldObject ignore, byte withFactionID, WorldObjectEqualityDelegate equalityDelegateOverride)
            => FindClosestBattler(position, ignore, withFactionID, equalityDelegateOverride);

        public int FindFactionCount() {
            int count = 0;

            foreach (var list in _factionBattlers) {
                count += list.Value.Count > 0 ? 1 : 0;
            }

            return count;
        }

        public int FindBattlerCountWithFaction(byte factionID) => _factionBattlers.ContainsKey(factionID) ? _factionBattlers[factionID].Count : 0;
    }
}
