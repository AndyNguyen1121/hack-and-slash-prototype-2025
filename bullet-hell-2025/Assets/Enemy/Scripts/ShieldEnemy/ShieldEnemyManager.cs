using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemyManager : EnemyManager
{
    [Header("Class-Specific Flags")]
    public bool isGuarding = true;
    public bool guardBroken;

    [Header("Shield Attributes")]
    public float shieldHealth = 50f;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    public override void Start()
    {
        base.Start();
    }
    private void Update()
    {
        animator.SetBool("isGuarding", isGuarding);
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
            }
            return;
        }

        base.TakeDamage(value, attackLocation, attackSource);
    }
}
