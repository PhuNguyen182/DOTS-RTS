using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                                                     .CreateCommandBuffer(state.WorldUnmanaged);
        foreach ((
            RefRW<LocalTransform> localTransform, 
            RefRO<Bullet> bullet, 
            RefRO<Target> target,
            Entity entity) 
            in SystemAPI.Query<
                RefRW<LocalTransform>, 
                RefRO<Bullet>, 
                RefRO<Target>>()
                .WithEntityAccess())
        {
            if(target.ValueRO.TargetEntity == Entity.Null)
            {
                commandBuffer.DestroyEntity(entity);
                continue;
            }

            LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
            ShootVictim shootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.TargetEntity);
            float3 targetPosition = targetTransform.TransformPoint(shootVictim.HitPosition);

            float distanceBeforeSqr = math.distancesq(localTransform.ValueRO.Position, targetPosition);
            
            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position = localTransform.ValueRO.Position 
                + moveDirection * bullet.ValueRO.Speed * SystemAPI.Time.DeltaTime;

            float distanceAfterSqr = math.distancesq(localTransform.ValueRO.Position, targetPosition);

            if (distanceAfterSqr > distanceBeforeSqr)
                localTransform.ValueRW.Position = targetPosition; // Overshoot

            float destroyDistanceSqr = 0.02f;
            if(math.distancesq(localTransform.ValueRO.Position, targetPosition) < destroyDistanceSqr)
            {
                RefRW<Health> health = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);
                health.ValueRW.HealthAmount -= bullet.ValueRO.Damage;
                commandBuffer.DestroyEntity(entity);
            }
        }
    }
}
