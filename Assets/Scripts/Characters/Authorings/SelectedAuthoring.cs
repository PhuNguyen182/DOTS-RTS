using Unity.Entities;
using UnityEngine;

public struct Selected : IComponentData, IEnableableComponent
{
    public float VisualScale;
    public Entity VisualEntity;
}

class SelectedAuthoring : MonoBehaviour
{
    public float VisualScale;
    public GameObject VisualGameObject;
}

class SelectedAuthoringBaker : Baker<SelectedAuthoring>
{
    public override void Bake(SelectedAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Selected
        {
            VisualScale = authoring.VisualScale,
            VisualEntity = GetEntity(authoring.VisualGameObject, TransformUsageFlags.Dynamic)
        });

        SetComponentEnabled<Selected>(entity, false);
    }
}
