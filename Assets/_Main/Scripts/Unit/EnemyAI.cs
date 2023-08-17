using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WAITING_FOR_ENEMY_TURN,
        TAKING_TURN,
        BUSY
    }

    private State state;
    private float timer;

    private void Awake()
    {
        state = State.WAITING_FOR_ENEMY_TURN;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.GetIsPlayerTurn())
        {
            state = State.TAKING_TURN;
            timer = 2f;
        }
    }

    private void Update()
    {
        if (TurnSystem.Instance.GetIsPlayerTurn()) return;

        switch (state)
        {
            case State.WAITING_FOR_ENEMY_TURN:
                break;
            case State.TAKING_TURN:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.BUSY;
                    }
                    else
                    {
                        //No more enemies have actions they can take, end enemy turn
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.BUSY:
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TAKING_TURN;
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.GetEnemyUnitList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        SpinAction spinAction = enemyUnit.GetSpinAction();

        GridPosition actionGridPosition = enemyUnit.GetGridPosition();

        if (!spinAction.IsValidActionGridPosition(actionGridPosition))
        {
            return false;
        }

        if (!enemyUnit.TrySpendActionPointsToTakeAction(spinAction))
        {
            return false;
        }

        spinAction.TakeAction(actionGridPosition, onEnemyAIActionComplete);
        return true;
    }

    private void OnDestroy()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }
}
