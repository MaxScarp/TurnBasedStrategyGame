using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy = false;

    private HealthSystem healthSystem;

    private GridPosition gridPosition;

    private int actionPoints;

    public bool IsEnemy { get => isEnemy; }
    public int ActionPoints { get => actionPoints; }
    public BaseAction[] BaseActionArray { get; private set; }

    private void Awake()
    {
        BaseActionArray = GetComponents<BaseAction>();

        actionPoints = ACTION_POINTS_MAX;

        if (!TryGetComponent(out HealthSystem healthSystem))
        {
            Debug.LogError($"Error: trying to get the component of type {typeof(HealthSystem)}! - {this.name}.");
            return;
        }
        this.healthSystem = healthSystem;
    }

    private void Start()
    {
        gridPosition = GameManager.LevelGrid.GetGridPosition(transform.position);
        GameManager.LevelGrid.AddUnitAtGridPosition(gridPosition, this);

        GameManager.TurnSystem.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        GameManager.LevelGrid.RemoveUnitAtGridPosition(gridPosition, this);

        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((!IsEnemy && GameManager.TurnSystem.IsPlayerTurn) || (IsEnemy && !GameManager.TurnSystem.IsPlayerTurn))
        {
            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Update()
    {
        GridPosition newGridPosition = GameManager.LevelGrid.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            //Unit changed GridPosition
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            GameManager.LevelGrid.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (!CanSpendActionPointsToTakeAction(baseAction)) return false;

        SpendActionPoints(baseAction.GetActionPointsCost());
        return true;
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction) => actionPoints >= baseAction.GetActionPointsCost();

    public GridPosition GetGridPosition() => gridPosition;
    public Vector3 GetWorldPosition() => transform.position;

    public float GetHealthNormalized() => healthSystem.HealthNormalized;

    public bool TryGetAction<T>(out T baseAction) where T : BaseAction
    {
        foreach (BaseAction baseActionTest in BaseActionArray)
        {
            if (baseActionTest is not T) continue;

            baseAction = (T)baseActionTest;
            return true;
        }

        baseAction = null;
        return false;
    }

    private void OnDestroy()
    {
        GameManager.TurnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged;
        healthSystem.OnDead -= HealthSystem_OnDead;
    }
}
