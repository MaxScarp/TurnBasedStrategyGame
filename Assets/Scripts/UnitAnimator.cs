using System;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string SHOOT = "Shoot";

    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;

    private MoveAction moveAction;
    private ShootAction shootAction;

    private void Awake()
    {
        if (!TryGetComponent(out MoveAction moveAction))
        {
            Debug.LogError($"Error: trying to get the component of type {typeof(MoveAction)}! - {this.name}.");
            return;
        }
        this.moveAction = moveAction;

        if (!TryGetComponent(out ShootAction shootAction))
        {
            Debug.LogError($"Error: trying to get the component of type {typeof(ShootAction)}! - {this.name}.");
            return;
        }
        this.shootAction = shootAction;
    }

    private void Start()
    {
        moveAction.OnStartMoving += MoveAction_OnStartMoving;
        moveAction.OnStopMoving += MoveAction_OnStopMoving;

        shootAction.OnShoot += ShootAction_OnShoot;
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger(SHOOT);

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        if (!bulletProjectileTransform.TryGetComponent(out BulletProjectile bulletProjectile))
        {
            Debug.LogError($"Error: trying to get the component of type {typeof(BulletProjectile)}! - {this.name}.");
            return;
        }

        Vector3 targetUnitShootAtPosition = e.TargetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = shootPointTransform.position.y;

        bulletProjectile.Setup(targetUnitShootAtPosition);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool(IS_WALKING, false);
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool(IS_WALKING, true);
    }

    private void OnDestroy()
    {
        moveAction.OnStartMoving -= MoveAction_OnStartMoving;
        moveAction.OnStopMoving -= MoveAction_OnStopMoving;

        shootAction.OnShoot -= ShootAction_OnShoot;
    }
}
