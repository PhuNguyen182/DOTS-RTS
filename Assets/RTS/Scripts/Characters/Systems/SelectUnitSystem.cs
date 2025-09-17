using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateBefore(typeof(ResetEventSystem))]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct SelectUnitSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
        {
            if (selected.ValueRO.OnSelected)
            {
                RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEntity);
                localTransform.ValueRW.Scale = selected.ValueRO.VisualScale;
            }

            else if (selected.ValueRO.OnDeselected)
            {
                RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEntity);
                localTransform.ValueRW.Scale = 0;
            }
        }
    }
}

[BurstCompile]
public partial struct SelectUnitJob : IJobEntity
{
    public void Execute(in Selected selected)
    {
        
    }
}
