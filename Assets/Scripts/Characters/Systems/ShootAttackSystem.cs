using Unity.Burst;
using Unity.Entities;
using Debug = UnityEngine.Debug;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<ShootAttack> shootAttack, RefRO<Target> target) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Target>>())
        {
            if (target.ValueRO.TargetEntity == Entity.Null)
                continue;

            shootAttack.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRW.Timer > 0)
                continue;

            shootAttack.ValueRW.Timer = shootAttack.ValueRO.TimerMax;
            RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);

            int attackDamage = 1;
            targetHealth.ValueRW.HealthAmount -= attackDamage;
        }
    }
}
