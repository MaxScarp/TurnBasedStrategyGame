using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.UnitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged;

        UpdateVisual(false);
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        UpdateVisual(isBusy);
    }

    private void UpdateVisual(bool isBusy)
    {
        gameObject.SetActive(isBusy);
    }
}
