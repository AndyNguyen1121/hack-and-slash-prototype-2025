using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleEnemyCombatManager : EnemyCombatManager
{
    public GameObject bullet;
    public Transform gunTip;
    public GameObject gunWarning;
    public override void OnWeaponTriggerEnter(Collider other)
    {
        return;
    }

    public override void Attack()
    {
        Instantiate(bullet, gunTip.position, Quaternion.LookRotation((PlayerManager.instance.playerCenter.transform.position - gunTip.position).normalized));
        
    }

    public void SpawnWarningIndicator()
    {
        Instantiate(gunWarning, gunTip.position, Quaternion.LookRotation((PlayerManager.instance.playerCenter.transform.position - gunTip.position).normalized));
    }
    public void PlayGunshotSFX()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.laserShoot, transform.position);
    }

    public override void CloseWeaponCollider()
    {
        return;
    }
}
