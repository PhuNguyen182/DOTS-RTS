using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

[UpdateBefore(typeof(BulletMoveSystem))]
partial struct UnitMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnitMoverJob unitMoverJob = new UnitMoverJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        unitMoverJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float DeltaTime;

    public void Execute(ref LocalTransform localTransform, in UnitMover unitMover, ref PhysicsVelocity physicsVelocity)
    {
        float3 targetPosition = unitMover.TargetPosition;
        float3 direction = targetPosition - localTransform.Position;

        if (math.lengthsq(direction) > 2f)
        {
            float rotationSpeed = unitMover.RotationSpeed;
            float3 moveDirection = math.normalize(direction);

            quaternion lookRotation = quaternion.LookRotation(moveDirection, math.up());
            quaternion targetRotation = math.slerp(localTransform.Rotation, lookRotation, DeltaTime * rotationSpeed);

            localTransform.Rotation = targetRotation;
            physicsVelocity.Linear = moveDirection * unitMover.MoveSpeed;
        }

        else
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }
    }
}
