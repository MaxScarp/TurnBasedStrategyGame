using System;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Vector3 targetPosition;
    private float moveSpeed = 15.0f;
    private float reachedTargetDistance = 0.2f;
    private float damageRadius = 4.0f;
    private int damage = 30;
    private Action onGrenadeBehaviourComplete;
    private float totalDistance;
    private Vector3 positionXZ;
    private float maxHeight;

    private void Update()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        maxHeight = totalDistance / 4.0f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        if (Vector3.Distance(positionXZ, targetPosition) <= reachedTargetDistance)
        {
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out Unit targetUnit))
                {
                    targetUnit.Damage(damage);
                }
                if (collider.TryGetComponent(out DestructibleCrate destructibleCrate))
                {
                    destructibleCrate.Damage();
                }
            }

            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            trailRenderer.transform.parent = null;

            Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up * 1.0f, Quaternion.identity);

            Destroy(gameObject);

            onGrenadeBehaviourComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        targetPosition = GameManager.LevelGrid.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
