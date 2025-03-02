using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ShootAttack : IComponentData
{
    public float Timer;
    public float TimerMax;
    public int Damage;
    public float AttackDistance;
    public float3 BulletSpawnPositionOffset;
}

class ShootAttackAuthoring : MonoBehaviour
{
    public float TimerMax;
    public int Damage;
    public float AttackDistance;
    public Transform BulletSpawnPoint;
}

class ShootAttackAuthoringBaker : Baker<ShootAttackAuthoring>
{
    public override void Bake(ShootAttackAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ShootAttack
        {
            TimerMax = authoring.TimerMax,
            Damage = authoring.Damage,
            AttackDistance = authoring.AttackDistance,
            BulletSpawnPositionOffset = authoring.BulletSpawnPoint.localPosition
        });
    }
}
