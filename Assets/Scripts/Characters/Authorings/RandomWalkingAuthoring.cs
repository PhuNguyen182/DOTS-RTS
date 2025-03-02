using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct RandomWalking : IComponentData
{
    public float3 TargetPosition;
    public float3 OriginPosition;
    public float MinDistance;
    public float MaxDistance;
    public Unity.Mathematics.Random Random;
}

class RandomWalkingAuthoring : MonoBehaviour
{
    public float3 TargetPosition;
    public float3 OriginPosition;
    public float MinDistance;
    public float MaxDistance;
}

class RandomWalkingAuthoringBaker : Baker<RandomWalkingAuthoring>
{
    public override void Bake(RandomWalkingAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new RandomWalking
        {
            TargetPosition = authoring.TargetPosition,
            OriginPosition = authoring.OriginPosition,
            MinDistance = authoring.MinDistance,
            MaxDistance = authoring.MaxDistance,
        });
    }
}
