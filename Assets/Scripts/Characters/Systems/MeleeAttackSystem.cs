using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct MeleeAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<LocalTransform> LocalTransform, RefRW<MeleeAttack> meleeAttack, RefRO<Target> target) 
            in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MeleeAttack>, RefRO<Target>>())
        {
            if (target.ValueRO.TargetEntity == Entity.Null)
                continue;


        }
    }
}
