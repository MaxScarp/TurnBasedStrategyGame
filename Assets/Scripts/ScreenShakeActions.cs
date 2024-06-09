using System;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private float grenadeShakeIntensity = 5.0f;
    private float swordShakeIntensity = 2.0f;

    private void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;
    }

    private void SwordAction_OnAnySwordHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(swordShakeIntensity);
    }

    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(grenadeShakeIntensity);
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake();
    }

    private void OnDestroy()
    {
        ShootAction.OnAnyShoot -= ShootAction_OnAnyShoot;
    }
}
