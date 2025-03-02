using Unity.Entities;
using UnityEngine;

class ZombieSpawnAuthoring : MonoBehaviour
{
    public float TimerMax;
    public float RandomWalkingDistanceMin;
    public float RandomWalkingDistanceMax;
}

class ZombieSpawnAuthoringBaker : Baker<ZombieSpawnAuthoring>
{
    public override void Bake(ZombieSpawnAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ZombieSpawn
        {
            TimerMax = authoring.TimerMax,
            RandomWalkingDistanceMin = authoring.RandomWalkingDistanceMin,
            RandomWalkingDistanceMax = authoring.RandomWalkingDistanceMax
        });
    }
}

public struct ZombieSpawn : IComponentData
{
    public float Timer;
    public float TimerMax;
    public float RandomWalkingDistanceMin;
    public float RandomWalkingDistanceMax;
}
