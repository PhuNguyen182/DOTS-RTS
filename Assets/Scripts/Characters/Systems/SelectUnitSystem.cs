using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SelectUnitSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>())
        {
            RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEntity);
            localTransform.ValueRW.Scale = 0;
        }

        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>())
        {
            RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEntity);
            localTransform.ValueRW.Scale = selected.ValueRO.VisualScale;
        }
    }
}
