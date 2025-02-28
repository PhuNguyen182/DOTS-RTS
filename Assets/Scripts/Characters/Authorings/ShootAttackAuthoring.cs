using Unity.Entities;
using UnityEngine;

public struct ShootAttack : IComponentData
{
    public float Timer;
    public float TimerMax;
    public int Damage;
}

class ShootAttackAuthoring : MonoBehaviour
{
    public float TimerMax;
    public int Damage;
}

class ShootAttackAuthoringBaker : Baker<ShootAttackAuthoring>
{
    public override void Bake(ShootAttackAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ShootAttack
        {
            TimerMax = authoring.TimerMax,
            Damage = authoring.Damage
        });
    }
}
