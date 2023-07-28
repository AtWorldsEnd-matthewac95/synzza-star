using System;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public class UnityWorld : World<UnityWorldObject> {
        private readonly BattlerNavAgentMonocomponent _player;

        public UnityWorld(SynzzaGame game, BattlerNavAgentMonocomponent player) : base(game) {
            _player = player;
        }

        protected override BattlerWorldObject FindClosestBattler(bool isUsingFactionID, byte factionIdToFind) {
            return !isUsingFactionID || factionIdToFind == SingletonSynzzaGame.Current.PlayerFactionID ? _player.WorldObject : null;
        }

        protected override UnityWorldObject CreateWorldObjectImpl(in IWorldObjectTemplate template, in float3 position, in float4 quaternion, out WorldObject optionallySpawnedObject) {
            if (template is not UnityPrefab prefab) {
                throw new ArgumentException($"Cannot instantiate non-{typeof(UnityPrefab).Name} template with {GetType().Name}!");
            }

            var t = UnityEngine.Object.Instantiate(prefab.Prefab, position.ToVector3(), quaternion.ToQuaternion()).transform;

            if (template is UnitySkillHitboxPrefab skillHitboxPrefab) {
                if (!t.gameObject.TryGetComponent(out SkillHitboxMonocomponent hitboxMono)) {
                    hitboxMono = t.gameObject.AddComponent<SkillHitboxMonocomponent>();
                }

                hitboxMono.Initialize(skillHitboxPrefab.SkillHitbox);
                optionallySpawnedObject = hitboxMono.WorldObject;
            } else {
                optionallySpawnedObject = null;
            }

            return new UnityWorldObject(t);
        }

        public override bool DestroyObject(in IMutableWorldObject obj) {
            if (obj is WorldObject transform) {
                UnityEngine.Object.Destroy(transform.GetImpl<UnityWorldObject>().transform.gameObject);
                return true;
            }

            return false;
        }

        public void ForceExit() {
            Exit();
        }
    }
}
