using Unity.Burst;
using Unity.Entities;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((RefRW<ShootAttack> shootAttack, RefRO<Target> target) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Target>>())
        {
            if (target.ValueRO.TargetEntity == Entity.Null)
                continue;

            shootAttack.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRW.Timer > 0)
                continue;

            shootAttack.ValueRW.Timer = shootAttack.ValueRO.TimerMax;
            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.BulletPrefabEntity);
        }
    }
}
