using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);

        float explosionForce = 300.0f;
        float explosionRange = 10.0f;

        Vector3 randomDir = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
        ApplyExplosionToRagdoll(ragdollRootBone, explosionForce, transform.position + randomDir, explosionRange);
    }

    private void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (!cloneChild) continue;

            cloneChild.position = child.position;
            cloneChild.rotation = child.rotation;

            MatchAllChildTransforms(child, cloneChild);
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (!child.TryGetComponent(out Rigidbody childRigidbody)) continue;

            childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);

            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
