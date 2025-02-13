using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
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

        for (int i = 0; i < _selectEntities.Length; i++)
            _entityManager.SetComponentEnabled<Selected>(_selectEntities[i], false);
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

        for (int i = 0; i < unitMovers.Length; i++)
        {
            UnitMover unitMover = unitMovers[i];
            unitMover.TargetPosition = mousePosition;
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
            if (_entityManager.HasComponent<Unit>(closestHit.Entity))
                _entityManager.SetComponentEnabled<Selected>(closestHit.Entity, true);
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
                _entityManager.SetComponentEnabled<Selected>(_selectEntities[i], true);
        }
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
