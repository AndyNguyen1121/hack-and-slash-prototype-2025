using Unity.Cinemachine;
using DG.Tweening;
using JetBrains.Annotations;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    public GameObject swordSpark;
    public GameObject shieldSpark;
    [Header("Initialization")]
    public Collider weaponCollider;
    public CinemachineImpulseSource camShake;
    private PlayerManager playerManager;

    [Header("Parry")]
    public float windowFrames;
    public float elapsedFrames;
    public bool parryWindowActive;
    public Transform parryParticleTransform;
    public ParticleSystem parryParticle;

    public float counterAttackWindowFrames;
    public float elapsedCounterAttackWindowFrames;
    public bool canCounterAttack;
    
    [Header("Damage Settings")]
    public float damage;
    public float knockUpForce;
    public float knockBackForce;
    public LayerMask whatIsDamageable;
    public bool canKnockUpGrounded;
    public bool canBreakShield;
    

    [Header("Attack Info")]
    public List<Collider> damagedEnemyColliders = new();

    [Header("HitStop")]
    public List<HitstopBaseScript> hitStopProfiles = new();
    public Coroutine hitStopCoroutine;


    // Cinemachine impulse default settings
    public float defaultImpulseFreq;
    public float defaultImpulseAmp;
    public float defaultImpulseDuration;
    private Coroutine impulseResetCoroutine;
    private void Start()
    {
        SetCamShakeDefaultValues();
        playerManager = PlayerManager.instance;
    }

    private void FixedUpdate()
    {
        HandleCounterAttackBehavior();
    }
    public void OnWeaponTriggerEnter(Collider collider)
    {
        if ((((1 << collider.gameObject.layer) & whatIsDamageable) != 0) && !damagedEnemyColliders.Contains(collider))
        {
            ActivateDefaultScreenShakeImpulse(-1);
            damagedEnemyColliders.Add(collider);
            ActivateHitStop(0);

            if (collider.gameObject.TryGetComponent<IDamageable>(out IDamageable component))
            {
                component.TakeDamage(damage, collider.ClosestPointOnBounds(weaponCollider.transform.position), gameObject);
                EnemyInteractionManager enemyInteractionManager;

                if (collider.TryGetComponent<EnemyInteractionManager>(out enemyInteractionManager))
                {
                    if (canKnockUpGrounded && enemyInteractionManager.isGrounded || 
                        !enemyInteractionManager.isGrounded)
                    {
                        enemyInteractionManager.JumpToHeightInTime(knockUpForce);
                    }

                    enemyInteractionManager.KnockBackRigidbody(knockBackForce, transform.position);
                }

                if (component is ShieldEnemyManager shieldManager && shieldManager.isGuarding)
                {
                    if (shieldManager.shieldHealth > 0 && canBreakShield)
                    {
                        shieldManager.ExplodeShield(); 
                    }
                    Instantiate(shieldSpark, collider.ClosestPoint(weaponCollider.transform.position), Quaternion.LookRotation(weaponCollider.transform.forward));
                }
                else
                {
                    Instantiate(swordSpark, collider.ClosestPoint(weaponCollider.transform.position), Quaternion.LookRotation(weaponCollider.transform.forward));
                }

                AudioManager.instance.PlayOneShot(FMODEvents.instance.swordHit, collider.ClosestPoint(weaponCollider.transform.position));
            }
        }
    }

    public void SetDamageValues(CombatScriptableObj stateInfo)
    {
        damage = stateInfo.damageInfo.damage;
        knockUpForce = stateInfo.damageInfo.knockUpForce;
        knockBackForce = stateInfo.damageInfo.knockBackForce;
        canKnockUpGrounded = stateInfo.damageInfo.knockUpGrounded;
    }

    public void HandleCounterAttackBehavior()
    {
        if (canCounterAttack && elapsedCounterAttackWindowFrames <= counterAttackWindowFrames)
        {
            elapsedCounterAttackWindowFrames += 1;
        }
        else
        {
            elapsedCounterAttackWindowFrames = 0;
            canCounterAttack = false;
        }
    }

    public void ActivateParryBehavior()
    {
        playerManager.playerCombatManager.canCounterAttack = true;
        Instantiate(parryParticle, parryParticleTransform.position, Quaternion.identity);
        PostProcessManager.instance.ActivateParryPostProcessingEffect();
        AudioManager.instance.PlayOneShot(FMODEvents.instance.swordParry, transform.position);
        ActivateDefaultScreenShakeImpulse(2f);
        ActivateHitStop(3);
    }


    public void ActivateDefaultScreenShakeImpulse(float force)
    {
        if (force < 0)
        {
            camShake.GenerateImpulseAt(transform.position, camShake.DefaultVelocity);
        }
        else
        {
            camShake.GenerateImpulseAt(transform.position, -Vector3.one * force);
        }

    }

    public void ActivateIntenseScreenShakeImpulse(float force)
    {
        if (impulseResetCoroutine != null)
        {
            StopCoroutine(impulseResetCoroutine);
        }

        camShake.ImpulseDefinition.FrequencyGain = 2f;
        camShake.ImpulseDefinition.AmplitudeGain = 1.5f;
        camShake.ImpulseDefinition.TimeEnvelope.SustainTime = 0.13f;

        if (force < 0)
        {
            camShake.GenerateImpulseAt(transform.position, camShake.DefaultVelocity);
        }
        else
        {
            camShake.GenerateImpulseAt(transform.position, -Vector3.one * force);
        }
        
         impulseResetCoroutine = StartCoroutine(ResetImpulseToDefaultValues(0.2f));
    }

    private void SetCamShakeDefaultValues()
    {
        defaultImpulseAmp = camShake.ImpulseDefinition.AmplitudeGain;
        defaultImpulseDuration = camShake.ImpulseDefinition.TimeEnvelope.SustainTime;
        defaultImpulseFreq = camShake.ImpulseDefinition.FrequencyGain;
    }

    private IEnumerator  ResetImpulseToDefaultValues(float time)
    {
        yield return new WaitForSeconds(time);

        camShake.ImpulseDefinition.AmplitudeGain = defaultImpulseAmp;
        camShake.ImpulseDefinition.FrequencyGain = defaultImpulseFreq;

        var envelope = camShake.ImpulseDefinition.TimeEnvelope;
        envelope.SustainTime = defaultImpulseDuration;
        camShake.ImpulseDefinition.TimeEnvelope = envelope;
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

    public void SetKnockUpDamage(float value)
    {
        knockUpForce = value;
    }

    public void SetDamage(float value)
    {
        damage = value;
    }

    public void SetKnockBackDamage(float value)
    {
        knockBackForce = value;
    }

    public void CanKnockUpGrounded()
    {
        canKnockUpGrounded = true;
    }

    public void CannotKnockupGrounded()
    {
        canKnockUpGrounded = false;
    }

    public void PlaySwordSlashSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.swordSlash, transform.position);
    }

    public void EnableInvincibility()
    {
        playerManager.playerHealthManager.isInvincible = true;
    }

    public void DisableInvincibility()
    {
        playerManager.playerHealthManager.isInvincible = false;
    }

    #endregion
}
