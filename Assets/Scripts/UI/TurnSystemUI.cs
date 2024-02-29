using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;

    private void Awake()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            GameManager.TurnSystem.NextTurn();

        });

        GameManager.TurnSystem.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Start()
    {
        UpdateTurnNumberText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnNumberText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnNumberText()
    {
        turnNumberText.text = $"TURN {GameManager.TurnSystem.TurnNumber + 1}";
    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisualGameObject.SetActive(!GameManager.TurnSystem.IsPlayerTurn);
    }

    private void UpdateEndTurnButtonVisibility()
    {
        endTurnButton.gameObject.SetActive(GameManager.TurnSystem.IsPlayerTurn);
    }

    private void OnDestroy()
    {
        GameManager.TurnSystem.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }
}
