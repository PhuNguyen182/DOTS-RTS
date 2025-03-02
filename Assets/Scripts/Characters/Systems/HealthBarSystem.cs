using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct HealthBarSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Vector3 cameraForward = Vector3.zero;
        if(Camera.main != null)
        {
            cameraForward = Camera.main.transform.forward;
        }

        foreach ((RefRW<LocalTransform> localTransform, RefRO<HealthBar> healthBar) in SystemAPI.Query<RefRW <LocalTransform>, RefRO <HealthBar>>())
        {
            LocalTransform parentLocalTransform = SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.Health);
            Quaternion lookUpRotation = quaternion.LookRotation(cameraForward, math.up());
            localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(lookUpRotation);

            Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.Health);
            float normalizedHealth = (float)health.HealthAmount / health.HealthMax;
            localTransform.ValueRW.Scale = normalizedHealth == 1 ? 0 : 1;
            
            RefRW<PostTransformMatrix> healthBarPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.BarVisual);
            healthBarPostTransformMatrix.ValueRW.Value = float4x4.Scale(normalizedHealth, 1, 1);
        }
    }
}
