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
    public float MaxHealth { get; set; }
    public UnityEvent OnHealthChanged { get; set; }

    public PlayerManager playerManager;
    public GameObject bloodParticle;
    public GameObject blockParticle;
    private bool playedDeathAnimation = false;

    private void Awake()
    {
        Health = MaxHealth;
    }

    private void Start()
    {
        playerManager = PlayerManager.instance;

        playerManager.playerUIManager.SetHealthSliderValue(1);
    }

    public bool TakeDamage(float value, Vector3 attackLocation, GameObject attackSource)
    {
        if (playerManager.playerCombatManager.parryWindowActive)
        {
            playerManager.playerCombatManager.ActivateParryBehavior();   
            return false;
        }
        else if (playerManager.animator.GetBool("isGuarding"))
        {
            value *= 0.5f;
        }

        Health = Mathf.Max(0, Health - value);

        playerManager.playerUIManager.UpdateHealthSliders(Health, MaxHealth);

        if (Health <= 0 && !playedDeathAnimation)
        {
            playedDeathAnimation = true;
            playerManager.playerAnimationManager.PlayActionAnimation("PlayerDeath", true);
            PlayerInputManager.instance.DisableAllInputsExceptUI();
            playerManager.isDead = true;
        }
        else if (Health > 0)
        {
            HandleHitBehavior(value, attackLocation, attackSource);
        }

        playerManager.swordSlashManager.DisableSwordSlashParticle();

        return true;
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

        if (!playerManager.animator.GetBool("isGuarding"))
        {
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
        }
        else
        {
            playerManager.playerAnimationManager.PlayActionAnimation("GuardHit", true);
        }

        GameObject particle = null;

        if (playerManager.animator.GetBool("isGuarding"))
        {
            particle = Instantiate(blockParticle, attackLocation + (0.3f * -hitDirection), Quaternion.identity);
        }
        else
        {
            particle = Instantiate(bloodParticle, attackLocation + (0.3f * -hitDirection), Quaternion.identity);
        }

        if (particle != null)
        {
            Destroy(particle, 1.5f);
        }

        playerManager.playerCombatManager.ActivateIntenseScreenShakeImpulse(1.5f);
    }

}
