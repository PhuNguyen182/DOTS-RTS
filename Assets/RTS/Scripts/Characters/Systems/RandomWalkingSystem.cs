using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomWalkingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<RandomWalking> randomWalking, RefRW<UnitMover> unitMover, RefRW<LocalTransform> localTransform) 
            in SystemAPI.Query<RefRW<RandomWalking>, RefRW<UnitMover>, RefRW<LocalTransform>>())
        {
            if(math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.TargetPosition) < 2)
            {
                Random random = randomWalking.ValueRO.Random;
                float3 randomDirection = new float3(random.NextFloat(-1f, +1f), 0, random.NextFloat(-1f, +1f));
                randomDirection = math.normalize(randomDirection);

                randomWalking.ValueRW.TargetPosition = randomWalking.ValueRO.OriginPosition
                                                       + randomDirection * random.NextFloat(randomWalking.ValueRO.MinDistance, randomWalking.ValueRO.MaxDistance);
                randomWalking.ValueRW.Random = random;
            }

            else
            {
                unitMover.ValueRW.TargetPosition = randomWalking.ValueRO.TargetPosition;
            }
        }
    }
}
