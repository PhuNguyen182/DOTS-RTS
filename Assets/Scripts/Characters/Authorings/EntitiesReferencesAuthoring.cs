using Unity.Entities;
using UnityEngine;

public struct EntitiesReferences : IComponentData
{
    public Entity BulletPrefabEntity;
    public Entity ZombiePrefabEntity;
}

class EntitiesReferencesAuthoring : MonoBehaviour
{
    public GameObject BulletPrefabGameObject;
    public GameObject ZombiePrefabGameObject;
}

class EntitiesReferencesAuthoringBaker : Baker<EntitiesReferencesAuthoring>
{
    public override void Bake(EntitiesReferencesAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new EntitiesReferences
        {
            BulletPrefabEntity = GetEntity(authoring.BulletPrefabGameObject, TransformUsageFlags.Dynamic),
            ZombiePrefabEntity = GetEntity(authoring.ZombiePrefabGameObject, TransformUsageFlags.Dynamic)
        });
    }
}
