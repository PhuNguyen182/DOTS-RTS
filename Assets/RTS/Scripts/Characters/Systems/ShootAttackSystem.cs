using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((RefRW<LocalTransform> localTransform, RefRW<ShootAttack> shootAttack, RefRO<Target> target, RefRW<UnitMover> unitMover)
            in SystemAPI.Query<RefRW<LocalTransform>, RefRW<ShootAttack>, RefRO<Target>, RefRW<UnitMover>>())
        {
            if (target.ValueRO.TargetEntity == Entity.Null)
                continue;

            shootAttack.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRW.Timer > 0)
                continue;

            shootAttack.ValueRW.Timer = shootAttack.ValueRO.TimerMax;
            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);

            if(math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > shootAttack.ValueRO.AttackDistance)
            {
                unitMover.ValueRW.TargetPosition = targetLocalTransform.Position;
                continue;
            }

            else
            {
                unitMover.ValueRW.TargetPosition = localTransform.ValueRO.Position;
            }

            float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
            aimDirection = math.normalize(aimDirection);

            quaternion aimRotation = quaternion.LookRotation(aimDirection, math.up());
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, aimRotation
                , SystemAPI.Time.DeltaTime * unitMover.ValueRO.RotationSpeed);

            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.BulletPrefabEntity);
            float3 bulletWorldSpawnPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.BulletSpawnPositionOffset);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletWorldSpawnPosition));

            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletBullet.ValueRW.Damage = shootAttack.ValueRO.Damage;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.TargetEntity = target.ValueRO.TargetEntity;
        }
    }
}
