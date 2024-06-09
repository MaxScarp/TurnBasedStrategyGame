using System;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    private GridPosition gridPosition;
    private bool isGreen;
    private bool isActive;
    private float timer;
    private Action onInteractionComplete;

    private void Start()
    {
        gridPosition = GameManager.LevelGrid.GetGridPosition(transform.position);
        GameManager.LevelGrid.SetInteractableAtGridPosition(gridPosition, this);

        SetColorGreen();
    }

    private void Update()
    {
        if (!isActive) return;

        timer -= Time.time;
        if (timer <= 0.0f)
        {
            isActive = false;
            onInteractionComplete();
        }
    }

    private void SetColorGreen()
    {
        isGreen = true;
        meshRenderer.material = greenMaterial;
    }

    private void SetColorRed()
    {
        isGreen = false;
        meshRenderer.material = redMaterial;
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;
        timer = 0.5f;

        if (isGreen)
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen();
        }
    }
}
