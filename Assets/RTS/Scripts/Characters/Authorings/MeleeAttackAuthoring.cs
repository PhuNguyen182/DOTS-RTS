using Unity.Entities;
using UnityEngine;

public struct MeleeAttack : IComponentData
{
    public float Timer;
    public float TimerMax;
    public int Damage;
    public float ColliderSize;
}

class MeleeAttackAuthoring : MonoBehaviour
{
    public int Damage;
    public float TimerMax;
    public float ColliderSize;
}

class MeleeAttackAuthoringBaker : Baker<MeleeAttackAuthoring>
{
    public override void Bake(MeleeAttackAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new MeleeAttack
        {
            TimerMax = authoring.TimerMax,
            Damage = authoring.Damage,
            ColliderSize = authoring.ColliderSize
        });
    }
}
