using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    }

    public void TakeDamage(float value, Vector3 attackLocation, GameObject attackSource)
    {
        value = -value;
        Health += value;

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

        Debug.Log($"Bro really got hit for {value} damage");

        playerManager.playerCombatManager.ActivateIntenseScreenShakeImpulse(1.5f);
    }

    public void IncreaseHealth(float value)
    {

    }

    public void SetHealthValue(float value)
    {
        Health = value;
    }

    

}
