using Unity.Entities;
using UnityEngine;

public struct HealthBar : IComponentData
{
    public Entity BarVisual;
    public Entity Health;
}

class HealthBarAuthoring : MonoBehaviour
{
    public GameObject BarVisual;
    public GameObject Health;
}

class HealthBarAuthoringBaker : Baker<HealthBarAuthoring>
{
    public override void Bake(HealthBarAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new HealthBar
        {
            BarVisual = GetEntity(authoring.BarVisual, TransformUsageFlags.NonUniformScale),
            Health = GetEntity(authoring.Health, TransformUsageFlags.Dynamic)
        });
    }
}
