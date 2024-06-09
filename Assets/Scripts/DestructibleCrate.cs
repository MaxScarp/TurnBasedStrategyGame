using System;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    public static event EventHandler OnAnyDestroied;

    [SerializeField] private Transform crateDestroyedPrefab;

    private GridPosition gridPosition;
    private float explosionForce;
    private float explosionRange;

    private void Start()
    {
        gridPosition = GameManager.LevelGrid.GetGridPosition(transform.position);
        explosionForce = 150.0f;
        explosionRange = 10.0f;
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (!child.TryGetComponent(out Rigidbody childRigidbody)) continue;

            childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }

    public void Damage()
    {
        Transform crateDestrouedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);

        ApplyExplosionToChildren(crateDestrouedTransform, explosionForce, transform.position, explosionRange);

        Destroy(gameObject);

        OnAnyDestroied?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition() => gridPosition;
}
