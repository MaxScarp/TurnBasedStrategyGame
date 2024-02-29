using System;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public event EventHandler OnTurnChanged;

    private int turnNumber;
    private bool isPlayerTurn;

    public int TurnNumber { get => turnNumber; }
    public bool IsPlayerTurn { get => isPlayerTurn; }

    private void Awake()
    {
        turnNumber = 0;
        isPlayerTurn = true;
    }

    public void NextTurn()
    {
        turnNumber++;
        isPlayerTurn = !isPlayerTurn;

        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }
}
