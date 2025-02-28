using Unity.Entities;
using UnityEngine;

public struct EntitiesReferences : IComponentData
{
    public Entity BulletPrefabEntity;
}

class EntitiesReferencesAuthoring : MonoBehaviour
{
    public GameObject BulletPrefabGameObject;
}

class EntitiesReferencesAuthoringBaker : Baker<EntitiesReferencesAuthoring>
{
    public override void Bake(EntitiesReferencesAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new EntitiesReferences
        {
            BulletPrefabEntity = GetEntity(authoring.BulletPrefabGameObject, TransformUsageFlags.Dynamic)
        });
    }
}
