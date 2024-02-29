using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedGameObject;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        textMeshPro.text = baseAction.GetActionName().ToUpper();

        button.onClick.AddListener(() =>
        {
            GameManager.UnitActionSystem.SelectedAction = baseAction;
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = GameManager.UnitActionSystem.SelectedAction;
        selectedGameObject.SetActive(selectedBaseAction == baseAction);
    }
}
