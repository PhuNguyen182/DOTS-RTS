using Unity.Entities;
using UnityEngine;

public struct Unit : IComponentData
{
    public Faction Faction;
}

class UnitAuthoring : MonoBehaviour
{
    public Faction Faction;
}

class UnitAuthoringBaker : Baker<UnitAuthoring>
{
    public override void Bake(UnitAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Unit
        {
            Faction = authoring.Faction
        });
    }
}
