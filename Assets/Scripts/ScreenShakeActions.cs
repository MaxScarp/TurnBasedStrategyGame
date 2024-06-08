using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private float grenadeShakeIntensity = 5.0f;

    private void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
    }

    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, System.EventArgs e)
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
