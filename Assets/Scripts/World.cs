namespace AWE.Synzza {
    public interface IWorld {
        IBattlerWorldObject FindClosestBattler(in float3 origin);
        IBattlerWorldObject FindClosestBattler(in float3 origin, byte withFactionID);

        IWorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position = default, in float3 rotation = default);
        IWorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in float3 position, in float4 quaternion);
        IWorldObject SpawnObjectFromTemplate(in IWorldObjectTemplate template, in IWorldObject spawner, in SpawnLocationProfile location);

        bool DestroyObject(in IWorldObject obj);

        public event WorldExitDelegate OnExit;
    }
}
