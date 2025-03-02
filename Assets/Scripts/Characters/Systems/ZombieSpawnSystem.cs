using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ZombieSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                                                           .CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((RefRW<LocalTransform> localTransform, RefRW<ZombieSpawn> zombieSpawn) 
            in SystemAPI.Query<RefRW<LocalTransform>, RefRW<ZombieSpawn>>())
        {
            zombieSpawn.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
            if (zombieSpawn.ValueRO.Timer > 0)
                continue;

            zombieSpawn.ValueRW.Timer = zombieSpawn.ValueRO.TimerMax;
            Entity zombie = state.EntityManager.Instantiate(entitiesReferences.ZombiePrefabEntity);
            SystemAPI.SetComponent(zombie, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            entityCommandBuffer.AddComponent(zombie, new RandomWalking
            {
                OriginPosition = localTransform.ValueRO.Position,
                TargetPosition = localTransform.ValueRO.Position,
                MinDistance = zombieSpawn.ValueRO.RandomWalkingDistanceMin,
                MaxDistance = zombieSpawn.ValueRO.RandomWalkingDistanceMax,
                Random = new Unity.Mathematics.Random((uint)zombie.Index)
            });
        }
    }
}
