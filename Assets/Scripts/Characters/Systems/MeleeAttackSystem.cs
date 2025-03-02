using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MeleeAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeList<RaycastHit> raycastHits = new(Allocator.Temp);
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorld.CollisionWorld;

        foreach ((RefRW<LocalTransform> localTransform, RefRW<MeleeAttack> meleeAttack, RefRO<Target> target, RefRW<UnitMover> unitMover)
            in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MeleeAttack>, RefRO<Target>, RefRW<UnitMover>>())
        {
            if (target.ValueRO.TargetEntity == Entity.Null)
                continue;

            float meleeAttackDistanceSq = 2f;
            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
            bool isCloseEnoughToAttack = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position) < meleeAttackDistanceSq;
            bool isTouchingTarget = false;

            if (!isCloseEnoughToAttack)
            {
                float3 dirToTarget = targetLocalTransform.Position - localTransform.ValueRO.Position;
                dirToTarget = math.normalize(dirToTarget);
                float rayCastDistance = 0.4f;

                RaycastInput raycastInput = new RaycastInput
                {
                    Start = localTransform.ValueRO.Position,
                    End = localTransform.ValueRO.Position + dirToTarget * (meleeAttack.ValueRO.ColliderSize + rayCastDistance),
                    Filter = CollisionFilter.Default
                };

                raycastHits.Clear();
                if (physicsWorld.CastRay(raycastInput, ref raycastHits))
                {
                    foreach (RaycastHit raycastHit in raycastHits)
                    {
                        if (raycastHit.Entity == target.ValueRO.TargetEntity)
                        {
                            isTouchingTarget = true;
                            break;
                        }
                    }
                }
            }

            if (!isCloseEnoughToAttack && !isTouchingTarget)
                unitMover.ValueRW.TargetPosition = targetLocalTransform.Position;

            else
            {
                unitMover.ValueRW.TargetPosition = localTransform.ValueRO.Position;
                meleeAttack.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                if (meleeAttack.ValueRO.Timer > 0)
                    continue;

                meleeAttack.ValueRW.Timer = meleeAttack.ValueRO.TimerMax;
                RefRW<Health> health = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);
                health.ValueRW.HealthAmount -= meleeAttack.ValueRO.Damage;
            }
        }
    }
}
