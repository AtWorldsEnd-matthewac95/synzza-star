using UnityEngine;
using System;

namespace AWE.Synzza.UnityLayer {
    /*
     * This script is configured to execute before the default time. In the Unity editor, go to Edit->Project Settings->Script Execution Order for more details.
     */
    public class WorldMonocomponent : MonoBehaviour, IWorld {
        [SerializeField] private BattlerMonocomponent _player;

        public event WorldExitDelegate OnExit;

        private void Start() {
            Debug.Assert(_player != null, $"{GetType().Name} {gameObject.name} does not have a reference to the player transform!");
            SingletonSynzzaGame.Current.SetCurrentWorld(this);
        }

        public IBattlerWorldObject FindClosestBattler(in float3 origin) => FindClosestBattler(isUsingFactionID: false, 0);
        public IBattlerWorldObject FindClosestBattler(in float3 origin, byte withFactionID) => FindClosestBattler(isUsingFactionID: true, factionIdToFind: withFactionID);

        private IBattlerWorldObject FindClosestBattler(bool isUsingFactionID, byte factionIdToFind) {
            return !isUsingFactionID || factionIdToFind == SingletonSynzzaGame.Current.PlayerFactionID ? _player.WorldObject : null;
        }

        public IWorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position, in float4 quaternion) {
            if (template is not UnityPrefab prefab) {
                throw new ArgumentException($"Cannot instantiate non-{typeof(UnityPrefab).Name} template with {GetType().Name}!");
            }

            var t = Instantiate(prefab.Prefab, position.ToVector3(), quaternion.ToQuaternion()).transform;

            if (template is UnitySkillHitboxPrefab skillHitboxPrefab) {
                if (!t.gameObject.TryGetComponent(out SkillHitboxMonocomponent hitboxMono)) {
                    hitboxMono = t.gameObject.AddComponent<SkillHitboxMonocomponent>();
                }

                hitboxMono.Initialize(skillHitboxPrefab.SkillHitbox);
                return hitboxMono.WorldObject;
            }

            return template switch {
                IBattlerWorldObjectTemplate battler => new UnityBattlerWorldObject(battler.Battler, t),
                _ => new UnityWorldObject(t)
            };
        }
        public IWorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position = default, in float3 rotation = default) {
            return SpawnObjectFromTemplate(template, position, Quaternion.Euler(rotation.ToVector3()).ToFloat4());
        }
        public IWorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in IWorldObject spawner, in SpawnLocationProfile location) {
            return SpawnObjectFromTemplate(template, location.FindSpawnPosition(spawner, spawner.WorldScale, spawner.WorldScale), location.FindSpawnRotation(spawner));
        }

        public bool DestroyObject(in IWorldObject obj) {
            if (obj is UnityWorldObject transform) {
                Destroy(transform.transform.gameObject);
                return true;
            }

            return false;
        }

        private void OnDestroy() {
            OnExit?.Invoke();
        }
    }
}
