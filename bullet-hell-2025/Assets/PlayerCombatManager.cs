using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    [Header("Initialization")]
    public Collider weaponCollider;
    public CinemachineImpulseSource camShake;
    private PlayerManager playerManager;
    
    [Header("DamageSettings")]
    public float damage;
    public LayerMask whatIsDamageable;

    [Header("Attack Info")]
    public List<Collider> damagedEnemyColliders = new();

    [Header("HitStop")]
    public List<HitstopBaseScript> hitStopProfiles = new();
    public Coroutine hitStopCoroutine;

    private void Start()
    {
        playerManager = PlayerManager.instance;
    }
    public void OnWeaponTriggerEnter(Collider collider)
    {
        if ((((1 << collider.gameObject.layer) & whatIsDamageable) != 0) && !damagedEnemyColliders.Contains(collider))
        {
            Debug.Log(collider.gameObject.name);
            camShake.GenerateImpulseAt(transform.position, camShake.m_DefaultVelocity);
            damagedEnemyColliders.Add(collider);
            ActivateHitStop(0);

            if (collider.gameObject.TryGetComponent<IDamageable>(out IDamageable component))
            {
                component.TakeDamage(damage);
            }
        }
    }

    #region HitStop
    
    public void ActivateHitStop(int profileIndex)
    {
        if (hitStopCoroutine != null)
        {
            StopCoroutine(hitStopCoroutine);
        }

        hitStopCoroutine = StartCoroutine(HitStop(hitStopProfiles[profileIndex]));
    }

    public IEnumerator HitStop(HitstopBaseScript profile)
    {
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(profile.hitStopDuration * profile.hitStopDelayPercentage);

        float timer = 0f;
        float duration = profile.hitStopDuration * (1 - ((profile.hitStopDuration * profile.hitStopDelayPercentage)));

        while (timer < duration)
        {
            Time.timeScale = Mathf.Lerp(0, 1, timer / duration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
    }

    #endregion

    #region Animation Events

    public void OpenWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    public void CloseWeaponCollider()
    {
        weaponCollider.enabled = false;
        damagedEnemyColliders.Clear();
    }

    #endregion
}
