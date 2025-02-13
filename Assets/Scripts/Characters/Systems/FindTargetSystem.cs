using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

        foreach ((
            RefRO<LocalTransform> localTransform, 
            RefRO<FindTarget> findTarget,
            RefRW<Target> target)
            in SystemAPI.Query<
                RefRO<LocalTransform>, 
                RefRO<FindTarget>,
                RefRW<Target>>())
        {
            distanceHits.Clear();
            CollisionFilter collisionFilter = new()
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UnitLayerMask,
                GroupIndex = 0
            };

            if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.Range, ref distanceHits, collisionFilter))
            {
                foreach (DistanceHit distanceHit in distanceHits)
                {
                    Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                    if(targetUnit.Faction == findTarget.ValueRO.TargetFaction)
                    {
                        target.ValueRW.TargetEntity = distanceHit.Entity;
                        break;
                    }
                }
            }
        }
    }
}
