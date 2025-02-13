using Unity.Entities;
using UnityEngine;

public struct FindTarget : IComponentData
{
    public float Range;
    public Faction TargetFaction;
    public float Timer;
    public float TimerMax;
}

class FindTargetAuthoring : MonoBehaviour
{
    public float Range;
    public Faction TargetFaction;
    public float TimerMax;
}

class FindTargetAuthoringBaker : Baker<FindTargetAuthoring>
{
    public override void Bake(FindTargetAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new FindTarget
        {
            Range = authoring.Range,
            TargetFaction = authoring.TargetFaction,
            TimerMax = authoring.TimerMax
        });
    }
}
