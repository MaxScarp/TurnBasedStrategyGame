using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        GameManager.UnitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        GameManager.UnitActionSystem.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        GameManager.UnitActionSystem.OnActionStarted += UnitActionSystem_OnActionStarted;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void CreateUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();

        Unit selectedUnit = GameManager.UnitActionSystem.SelectedUnit;
        foreach (BaseAction baseAction in selectedUnit.BaseActionArray)
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            if (!actionButtonTransform.TryGetComponent(out ActionButtonUI actionButtonUI))
            {
                Debug.LogError($"Error: Trying to take component of type {typeof(ActionButtonUI)}! - {this.name}.");
                return;
            }
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
        actionPointsText.text = $"Action Points: {GameManager.UnitActionSystem.SelectedUnit.ActionPoints}";
    }

    private void OnDestroy()
    {
        GameManager.UnitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        GameManager.UnitActionSystem.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
        GameManager.UnitActionSystem.OnActionStarted -= UnitActionSystem_OnActionStarted;
        Unit.OnAnyActionPointsChanged -= Unit_OnAnyActionPointsChanged;
    }
}
