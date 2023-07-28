namespace AWE.Synzza {
    public interface IWorld {
        IBattlerWorldCatalog Battlers { get; }

        event WorldExitDelegate OnExit;

        IBattlerWorldObject FindClosestBattler(in float3 origin);
        IBattlerWorldObject FindClosestBattler(in float3 origin, byte withFactionID);

        IMutableWorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position = default, in float3 rotation = default);
        IMutableWorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position, in float4 quaternion);
        IMutableWorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in IWorldObject spawner, in SpawnLocationProfile location);

        bool DestroyObject(in IMutableWorldObject obj);
    }

    public interface IWorld<TWorldObjectImpl> : IWorld where TWorldObjectImpl : WorldObject.Impl {
        new IBattlerWorldCatalog<BattlerWorldObject> Battlers { get; }

        new BattlerWorldObject FindClosestBattler(in float3 origin);
        new BattlerWorldObject FindClosestBattler(in float3 origin, byte withFactionID);

        new WorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position = default, in float3 rotation = default);
        new WorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position, in float4 quaternion);
        new WorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in IWorldObject spawner, in SpawnLocationProfile location);
    }

    public abstract class World<TWorldObjectImpl> : SynzzaGameDependent, IWorld<TWorldObjectImpl> where TWorldObjectImpl : WorldObject.Impl {
        public BattlerWorldCatalog<BattlerWorldObject> Battlers { get; }
        IBattlerWorldCatalog<BattlerWorldObject> IWorld<TWorldObjectImpl>.Battlers => Battlers;
        IBattlerWorldCatalog IWorld.Battlers => Battlers;

        public event WorldExitDelegate OnExit;

        public World(SynzzaGame game) : base(game) {
            game.SetCurrentWorld(this);
            Battlers = new();
        }

        public BattlerWorldObject FindClosestBattler(in float3 origin) => FindClosestBattler(isUsingFactionID: false, 0);
        public BattlerWorldObject FindClosestBattler(in float3 origin, byte withFactionID) => FindClosestBattler(isUsingFactionID: true, factionIdToFind: withFactionID);

        IBattlerWorldObject IWorld.FindClosestBattler(in float3 origin) => FindClosestBattler(origin);
        IBattlerWorldObject IWorld.FindClosestBattler(in float3 origin, byte withFactionID) => FindClosestBattler(origin, withFactionID);

        protected abstract BattlerWorldObject FindClosestBattler(bool isUsingFactionID, byte factionIdToFind);

        protected abstract TWorldObjectImpl CreateWorldObjectImpl(in IWorldObjectTemplate template, in float3 position, in float4 quaternion, out WorldObject optionallySpawnedObject);

        public WorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position, in float4 quaternion) {
            var impl = CreateWorldObjectImpl(template, position, quaternion, out var optionallySpawnedObject);

            return optionallySpawnedObject ?? template switch {
                IBattlerWorldObjectTemplate battler => new BattlerWorldObject(impl, battler.Battler),
                _ => new WorldObject(impl)
            };
        }
        public WorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position = default, in float3 rotation = default)
            => SpawnObjectFromTemplate(template, position, QuaternionMath.Calculator.Euler(rotation));
        public WorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in IWorldObject spawner, in SpawnLocationProfile location)
            => SpawnObjectFromTemplate(template, location.FindSpawnPosition(spawner, spawner.WorldScale, spawner.WorldScale), location.FindSpawnRotation(spawner));

        IMutableWorldObject IWorld.SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position, in float3 rotation) => SpawnObjectFromTemplate(template, position, rotation);
        IMutableWorldObject IWorld.SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position, in float4 quaternion) => SpawnObjectFromTemplate(template, position, quaternion);
        IMutableWorldObject IWorld.SpawnObjectFromTemplate(in IWorldObjectTemplate template, in IWorldObject spawner, in SpawnLocationProfile location) => SpawnObjectFromTemplate(template, spawner, location);

        public abstract bool DestroyObject(in IMutableWorldObject obj);

        protected void Exit() {
            OnExit?.Invoke();
        }
    }
}
