using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WAITING_FOR_ENEMY_TURN,
        TAKING_TURN,
        BUSY,
    }

    private State state;
    private float timer;

    private void Awake()
    {
        state = State.WAITING_FOR_ENEMY_TURN;
    }

    private void Start()
    {
        GameManager.TurnSystem.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (GameManager.TurnSystem.IsPlayerTurn) return;

        timer = 2.0f;
        state = State.TAKING_TURN;
    }

    private void Update()
    {
        if (GameManager.TurnSystem.IsPlayerTurn) return;

        switch (state)
        {
            case State.WAITING_FOR_ENEMY_TURN:
                break;
            case State.TAKING_TURN:
                timer -= Time.deltaTime;
                if (timer <= 0.0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.BUSY;
                    }
                    else
                    {
                        GameManager.TurnSystem.NextTurn();
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
        foreach (Unit enemyUnit in GameManager.UnitManager.EnemyUnitList)
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete)) return true;
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in enemyUnit.BaseActionArray)
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction)) continue;

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.ActionValue > bestEnemyAIAction.ActionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if (bestEnemyAIAction == null || !enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction)) return false;

        bestBaseAction.TakeAction(bestEnemyAIAction.GridPosition, onEnemyAIActionComplete);

        return true;
    }

    private void OnDestroy()
    {
        GameManager.TurnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }
}
