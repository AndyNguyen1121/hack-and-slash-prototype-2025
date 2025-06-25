using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class PlayerHealthManager : MonoBehaviour, IDamageable
{
    [field: SerializeField]
    public float Health { get; private set; }

    [field: SerializeField]
    public float MaxHealth { get; private set; }
    public UnityEvent OnHealthChanged { get; set; }

    public PlayerManager playerManager;
    public GameObject bloodParticle;

    private void Awake()
    {
        Health = MaxHealth;
    }

    private void Start()
    {
        playerManager = PlayerManager.instance;

        playerManager.playerUIManager.SetHealthSliderValue(1);
    }

    public void TakeDamage(float value, Vector3 attackLocation, GameObject attackSource)
    {
        if (playerManager.playerCombatManager.parryWindowActive)
        {
            playerManager.playerCombatManager.ActivateParryBehavior();   
            return;
        }
        value = -value;
        Health += value;

        playerManager.playerUIManager.UpdateHealthSliders(Health, MaxHealth);

        HandleHitBehavior(value, attackLocation, attackSource);

        playerManager.swordSlashManager.DisableSwordSlashParticle();
    }

    public void IncreaseHealth(float value)
    {
        playerManager.playerUIManager.UpdateHealthSliders(Health, MaxHealth);
    }

    public void SetHealthValue(float value)
    {
        Health = value;
        playerManager.playerUIManager.UpdateHealthSliders(Health, MaxHealth);
    }

    public void HandleHitBehavior(float value, Vector3 attackLocation, GameObject attackSource)
    {
        Vector3 hitDirection = attackLocation - transform.position;
        hitDirection.y = 0;

        float horizontalHitDir = Vector3.Dot(hitDirection.normalized, transform.right);
        float verticalHitDir = Vector3.Dot(transform.forward, hitDirection.normalized);


        if (Mathf.Abs(horizontalHitDir) > Mathf.Abs(verticalHitDir))
        {
            if (horizontalHitDir < 0)
            {
                playerManager.playerAnimationManager.PlayActionAnimation("DamageLeft", true);
            }
            else
            {
                playerManager.playerAnimationManager.PlayActionAnimation("DamageRight", true);
            }
        }
        else
        {
            if (verticalHitDir < 0)
            {
                playerManager.playerAnimationManager.PlayActionAnimation("DamageBack", true);
            }
            else
            {
                playerManager.playerAnimationManager.PlayActionAnimation("DamageFront", true);
            }
        }

        Instantiate(bloodParticle, attackLocation + (0.3f * -hitDirection), Quaternion.identity);

        playerManager.playerCombatManager.ActivateIntenseScreenShakeImpulse(1.5f);
    }

}
