using Unity.Entities;
using UnityEngine;

public struct Bullet : IComponentData
{
    public float Speed;
    public int Damage;
}

class BulletAuthoring : MonoBehaviour
{
    public float Speed;
    public int Damage;
}

class BulletAuthoringBaker : Baker<BulletAuthoring>
{
    public override void Bake(BulletAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Bullet
        {
            Speed = authoring.Speed,
            Damage = authoring.Damage
        });
    }
}
