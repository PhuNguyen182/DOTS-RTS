using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial struct ResetTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
        {
            if (target.ValueRO.TargetEntity != Entity.Null)
            {
                if (!SystemAPI.Exists(target.ValueRO.TargetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.TargetEntity))
                    target.ValueRW.TargetEntity = Entity.Null;
            }
        }
    }
}
