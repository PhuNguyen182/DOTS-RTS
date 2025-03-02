using Unity.Entities;
using UnityEngine;

public struct Health : IComponentData
{
    public int HealthAmount;
    public int HealthMax;
}

class HealthAuthoring : MonoBehaviour
{
    public int HealthAmount;
    public int HealthMax;
}

class HealthAuthoringBaker : Baker<HealthAuthoring>
{
    public override void Bake(HealthAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Health
        {
            HealthAmount = authoring.HealthAmount,
            HealthMax = authoring.HealthMax
        });
    }
}
