using System;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private const string OPEN = "IsOpen";

    [SerializeField] private bool isOpen = false;

    private GridPosition gridPosition;
    private Animator animator;
    private Action onInteractionComplete;
    private float timer;
    private bool isActive;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gridPosition = GameManager.LevelGrid.GetGridPosition(transform.position);
        GameManager.LevelGrid.SetInteractableAtGridPosition(gridPosition, this);

        GameManager.Pathfinding.OnPathfindingSetted += Pathfinding_OnPathfindingSetted;
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

    private void Pathfinding_OnPathfindingSetted(object sender, EventArgs e)
    {
        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool(OPEN, true);
        GameManager.Pathfinding.SetIsWalkable(gridPosition, true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool(OPEN, false);
        GameManager.Pathfinding.SetIsWalkable(gridPosition, false);
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;
        timer = 0.5f;

        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }
}
