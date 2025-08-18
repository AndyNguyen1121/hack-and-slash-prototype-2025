using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemyManager : EnemyManager
{
    [Header("Class-Specific Flags")]
    public bool isGuarding = true;
    public bool guardBroken;

    [Header("Shield Attributes")]
    public GameObject shield;
    public GameObject shieldFractureGameObject;
    public float explosionForce = 2f;
    public float explosionHeight;
    public float shieldHealth = 50f;

    private Vector3 previousPosition;
    private float velocity;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        canGrapple = false;
    }

    // Update is called once per frame
    public override void Start()
    {
        base.Start();
    }
    private void Update()
    {
        animator.SetBool("isGuarding", isGuarding);

        
        velocity = ((transform.position - previousPosition) / Time.deltaTime).magnitude;

        if (velocity > 50f)
        {
            enemyCollider.isTrigger = true;
        }
        else
        {
            enemyCollider.isTrigger = false;
        }

        previousPosition = transform.position;
    }

    public override void TakeDamage(float value, Vector3 attackLocation, GameObject attackSource)
    {
        if (isGuarding)
        {
            shieldHealth = Mathf.Max(shieldHealth - value, 0);

            if (shieldHealth == 0 && enemyBehavior is ShieldEnemyBehavior shieldEnemyBehavior)
            {
                shieldEnemyBehavior.UnparentShield();
                isGuarding = false;
                guardBroken = true;
                canGrapple = true;
                ExplodeShield();
                return;
            }

            enemyAnimationManager.PlayActionAnimation("TinyImpact", false, true, true, 0);
            
            return;
        }

        base.TakeDamage(value, attackLocation, attackSource);
    }

    private void ExplodeShield()
    {
        if (shield == null)
            return;
        
        enemyAnimationManager.PlayActionAnimation("BigImpact");

        GameObject fracturedShield = Instantiate(shieldFractureGameObject, shield.transform.position, shield.transform.rotation);

        Rigidbody[] fracturedRigidbodies = fracturedShield.GetComponentsInChildren<Rigidbody>();
        Vector3 explosionPos = shield.transform.position;
        Debug.Log(fracturedRigidbodies.Length);

        foreach (Rigidbody rb in fracturedRigidbodies)
        {
            rb.AddExplosionForce(explosionForce, explosionPos, 2f, explosionHeight);
            Destroy(rb.gameObject, Random.Range(3, 6));
        }
        Destroy(fracturedShield, 7);
        shield.SetActive(false);
        PlayerManager.instance.playerCombatManager.ActivateHitStop(4);
    }
}
