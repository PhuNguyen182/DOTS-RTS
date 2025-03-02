using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    private const float RingSize = 2f;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float minMultipleSelectSize = 50;
    [SerializeField] private LayerMask unitMask;

    private float _selectSize;
    private bool _isMultipleSelect;
    private Vector2 _selectedStartPointerPosition;

    private EntityQuery _entityQuery;
    private NativeArray<Entity> _selectEntities;
    private EntityManager _entityManager;

    public event EventHandler OnSelectAreaStart;
    public event EventHandler OnSelectAreaEnd;

    public static UnitSelectionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _selectedStartPointerPosition = Input.mousePosition;
            OnSelectAreaStart?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            DisableSelectedUnits();

            _entityQuery = new EntityQueryBuilder(Allocator.Temp)
                                .WithAll<LocalTransform, Unit>()
                                .WithPresent<Selected>()
                                .Build(_entityManager);

            Rect selectArea = GetSelectArea();
            _selectSize = selectArea.width + selectArea.height;
            _isMultipleSelect = _selectSize > minMultipleSelectSize;

            if (_isMultipleSelect)
                SelectMultipleUnit(selectArea);

            else
                SelectSingleUnit();

            OnSelectAreaEnd?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonDown(1))
            MoveSelectedUnits();
    }

    private void DisableSelectedUnits()
    {
        _entityQuery = new EntityQueryBuilder(Allocator.Temp)
                                            .WithAll<Selected>()
                                            .Build(_entityManager);
        _selectEntities = _entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<Selected> selecteds = _entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

        for (int i = 0; i < _selectEntities.Length; i++)
        {
            _entityManager.SetComponentEnabled<Selected>(_selectEntities[i], false);
            Selected selected = selecteds[i];
            selected.OnDeselected = true;
            _entityManager.SetComponentData(_selectEntities[i], selected);
        }
    }

    private void MoveSelectedUnits()
    {
        Vector3 mousePosition = PointerInput.Instance.GetMousePosition();
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                                      .WithAll<UnitMover, Selected>()
                                      .Build(entityManager);

        NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<UnitMover> unitMovers = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
        NativeArray<float3> unitPositions = GetGenerateMovePositions(mousePosition, unitMovers.Length);

        for (int i = 0; i < unitMovers.Length; i++)
        {
            UnitMover unitMover = unitMovers[i];
            unitMover.TargetPosition = unitPositions[i];
            unitMovers[i] = unitMover;
        }

        entityQuery.CopyFromComponentDataArray(unitMovers);
    }

    private void SelectSingleUnit()
    {
        _entityQuery = _entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
        PhysicsWorldSingleton physicsWorld = _entityQuery.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorld.CollisionWorld;

        UnityEngine.Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastInput raycastInput = new RaycastInput
        {
            Start = cameraRay.GetPoint(0),
            End = cameraRay.GetPoint(200f),
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = (uint)unitMask.value,
                GroupIndex = 0
            }
        };

        if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit closestHit))
        {
            if (_entityManager.HasComponent<Unit>(closestHit.Entity) && _entityManager.HasComponent<Selected>(closestHit.Entity))
            {
                _entityManager.SetComponentEnabled<Selected>(closestHit.Entity, true);
                Selected selected = _entityManager.GetComponentData<Selected>(closestHit.Entity);
                selected.OnSelected = true;
                _entityManager.SetComponentData(closestHit.Entity, selected);
            }
        }
    }

    private void SelectMultipleUnit(Rect selectArea)
    {
        _selectEntities = _entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<LocalTransform> localTransforms = _entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

        for (int i = 0; i < localTransforms.Length; i++)
        {
            LocalTransform localTransform = localTransforms[i];
            Vector2 unitScreenPosition = mainCamera.WorldToScreenPoint(localTransform.Position);

            if (selectArea.Contains(unitScreenPosition))
            {
                _entityManager.SetComponentEnabled<Selected>(_selectEntities[i], true);
                Selected selected = _entityManager.GetComponentData<Selected>(_selectEntities[i]);
                selected.OnSelected = true;
                _entityManager.SetComponentData(_selectEntities[i], selected);
            }
        }
    }

    private NativeArray<float3> GetGenerateMovePositions(float3 targetPosition, int positionCount)
    {
        NativeArray<float3> movePositions = new NativeArray<float3>(positionCount, Allocator.Temp);
        
        if (positionCount == 0)
            return movePositions;
        
        else if(positionCount == 1)
        {
            movePositions[0] = targetPosition;
            return movePositions;
        }

        int ringCount = 0;
        int positionIndex = 0;

        while (positionIndex < positionCount)
        {
            int ringPositionCount = 3 + ringCount * 2;
            for (int i = 0; i < ringPositionCount; i++)
            {
                float angle = i * (math.PI2 / ringPositionCount);
                quaternion rotation = quaternion.RotateY(angle);
                float3 movePosition = new float3(RingSize * (ringCount + 1), 0, 0);
                float3 ringPosition = targetPosition + math.rotate(rotation, movePosition);

                movePositions[positionIndex] = ringPosition;
                positionIndex = positionIndex + 1;

                if (positionIndex >= positionCount)
                    break;
            }

            ringCount = ringCount + 1;
        }

        return movePositions;
    }

    public Rect GetSelectArea()
    {
        Vector2 selectedEndPointerPosition = Input.mousePosition;

        Vector2 bottomLeft = new Vector2
        {
            x = Mathf.Min(_selectedStartPointerPosition.x, selectedEndPointerPosition.x),
            y = Mathf.Min(_selectedStartPointerPosition.y, selectedEndPointerPosition.y)
        };

        Vector2 topRight = new Vector2
        {
            x = Mathf.Max(_selectedStartPointerPosition.x, selectedEndPointerPosition.x),
            y = Mathf.Max(_selectedStartPointerPosition.y, selectedEndPointerPosition.y)
        };

        return new Rect
        {
            x = bottomLeft.x,
            y = bottomLeft.y,
            width = topRight.x - bottomLeft.x,
            height = topRight.y - bottomLeft.y
        };
    }
}
