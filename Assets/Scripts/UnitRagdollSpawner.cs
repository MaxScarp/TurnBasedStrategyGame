using System;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform ragdollPrefab;
    [SerializeField] private Transform originalRootBone;

    private HealthSystem healthSystem;

    private void Awake()
    {
        if (!TryGetComponent(out HealthSystem healthSystem))
        {
            Debug.LogError($"Error: trying to get the component of type {typeof(HealthSystem)}! - {this.name}.");
            return;
        }
        this.healthSystem = healthSystem;
    }

    private void Start()
    {
        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Transform ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        if (!ragdollTransform.TryGetComponent(out UnitRagdoll unitRagdoll))
        {
            Debug.LogError($"Error: trying to get the component of type {typeof(UnitRagdoll)}! - {this.name}.");
            return;
        }
        unitRagdoll.Setup(originalRootBone);
    }

    private void OnDestroy()
    {
        healthSystem.OnDead -= HealthSystem_OnDead;
    }
}
