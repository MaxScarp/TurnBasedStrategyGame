using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        if (!TryGetComponent(out MeshRenderer meshRenderer))
        {
            Debug.LogError($"Error: Trying to get component {typeof(MeshRenderer)}! - {name}.");
            return;
        }

        this.meshRenderer = meshRenderer;
    }

    private void Start()
    {
        GameManager.UnitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;

        UpdateVisual();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (GameManager.UnitActionSystem.SelectedUnit == unit)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    private void OnDestroy()
    {
        GameManager.UnitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
    }
}
