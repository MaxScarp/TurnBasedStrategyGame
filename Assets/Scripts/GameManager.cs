using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GAME")]
    [SerializeField] private Transform[] componentTransformArray;
    [SerializeField] private Transform[] UIComponentTransformArray;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera actionVirtualCamera;
    [SerializeField] private Transform canvasTransform;

    [Header("DEBUG")]
    [SerializeField] private bool isDebugMode;
    [SerializeField] private bool isThereOneUnitWithoutSpinAction;
    [SerializeField] private bool isThereEnemies;
    [SerializeField] private Transform unitTransform;
    [SerializeField] private Transform unitWithoutSpinActionTransform;
    [SerializeField] private Transform enemyTransform;
    [SerializeField, Range(0, 5)] private int unitAmount;
    [SerializeField, Range(0, 5)] private int enemyAmount;
    [SerializeField] private List<Vector3> unitPositionList;
    [SerializeField] private Vector3 unitWithoutSpinActionPosition;
    [SerializeField] private List<Vector3> enemyPositionList;
    [SerializeField] private Transform componenetTransformTest;

    public static Vector3 MousePosition { get; private set; }
    public static MouseWorld MouseWorld { get; private set; }
    public static UnitActionSystem UnitActionSystem { get; private set; }
    public static LevelGrid LevelGrid { get; private set; }
    public static CameraController CameraController { get; private set; }
    public static CameraManager CameraManager { get; private set; }
    public static CinemachineVirtualCamera CinemachineVirtualCamera { get; private set; }
    public static CinemachineVirtualCamera ActionVirtualCamera { get; private set; }
    public static GridSystemVisual GridSystemVisual { get; private set; }
    public static UnitActionSystemUI UnitActionSystemUI { get; private set; }
    public static ActionBusyUI ActionBusyUI { get; private set; }
    public static TurnSystem TurnSystem { get; private set; }
    public static TurnSystemUI TurnSystemUI { get; private set; }
    public static EnemyAI EnemyAI { get; private set; }
    public static UnitManager UnitManager { get; private set; }
    public static Pathfinding Pathfinding { get; private set; }

    private void Awake()
    {
        CinemachineVirtualCamera = cinemachineVirtualCamera;
        ActionVirtualCamera = actionVirtualCamera;

        foreach (Transform componentTransform in componentTransformArray)
        {
            Transform componentTransformInstance = Instantiate(componentTransform, transform);

            if (componentTransformInstance.TryGetComponent(out MouseWorld mouseWorld))
            {
                MouseWorld = mouseWorld;
            }
            else if (componentTransformInstance.TryGetComponent(out LevelGrid levelGrid))
            {
                LevelGrid = levelGrid;
            }
            else if (componentTransformInstance.TryGetComponent(out UnitActionSystem unitActionSystem))
            {
                UnitActionSystem = unitActionSystem;
            }
            else if (componentTransformInstance.TryGetComponent(out CameraController cameraController))
            {
                CameraController = cameraController;
            }
            else if (componentTransformInstance.TryGetComponent(out CameraManager cameraManager))
            {
                CameraManager = cameraManager;
            }
            else if (componentTransformInstance.TryGetComponent(out TurnSystem turnSystem))
            {
                TurnSystem = turnSystem;
            }
            else if (componentTransformInstance.TryGetComponent(out EnemyAI enemyAI))
            {
                EnemyAI = enemyAI;
            }
            else if (componentTransformInstance.TryGetComponent(out GridSystemVisual gridSystemVisual))
            {
                GridSystemVisual = gridSystemVisual;
            }
            else if (componentTransformInstance.TryGetComponent(out UnitManager unitManager))
            {
                UnitManager = unitManager;
            }
            else if (componentTransformInstance.TryGetComponent(out Pathfinding pathfinding))
            {
                Pathfinding = pathfinding;
            }
        }
    }

    private void Start()
    {
        foreach (Transform UIcomponentTransform in UIComponentTransformArray)
        {
            Transform UIcomponentTransformInstance = Instantiate(UIcomponentTransform, canvasTransform);

            if (UIcomponentTransformInstance.TryGetComponent(out UnitActionSystemUI unitActionSystemUI))
            {
                UnitActionSystemUI = unitActionSystemUI;
            }
            else if (UIcomponentTransformInstance.TryGetComponent(out ActionBusyUI actionBusyUI))
            {
                ActionBusyUI = actionBusyUI;
            }
            else if (UIcomponentTransformInstance.TryGetComponent(out TurnSystemUI turnSystemUI))
            {
                TurnSystemUI = turnSystemUI;
            }
        }

        if (!isDebugMode) return;

        if (unitAmount > unitPositionList.Count)
        {
            Debug.LogError($"Error: Too many units for spawn position amount! - {this.name}.");
            return;
        }

        for (int i = 0; i < unitAmount; i++)
        {
            Transform unitTransformInstance = Instantiate(unitTransform, unitPositionList[i], Quaternion.identity);
            if (i == 0)
            {
                UnitActionSystem.SelectedUnit = unitTransformInstance.GetComponent<Unit>();
            }
        }

        if (isThereOneUnitWithoutSpinAction)
        {
            Instantiate(unitWithoutSpinActionTransform, unitWithoutSpinActionPosition, Quaternion.identity);
        }

        if (isThereEnemies)
        {
            if (enemyAmount > enemyPositionList.Count)
            {
                Debug.LogError($"Error: Too many enemies for spawn position amount! - {this.name}.");
                return;
            }

            for (int i = 0; i < enemyAmount; i++)
            {
                Instantiate(enemyTransform, enemyPositionList[i], enemyTransform.rotation);
            }
        }

        Instantiate(componenetTransformTest, transform);
    }

    private void Update()
    {
        MousePosition = MouseWorld.GetPosition();
    }
}
