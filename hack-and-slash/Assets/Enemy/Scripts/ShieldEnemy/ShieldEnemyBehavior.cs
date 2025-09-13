using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemyBehavior : EnemyBehavior
{
    ShieldEnemyManager shieldEnemyManager;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
        if (enemyManager is ShieldEnemyManager shieldEnemyManager)
        {
            this.shieldEnemyManager = shieldEnemyManager;
        }

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    private bool shieldUnparented;
    public override void StartDeathSequence()
    {
        base.StartDeathSequence();
        if (!shieldUnparented)
        {
            UnparentShield();
            shieldUnparented = true;
        }
    }

    public void UnparentShield()
    {
       
        shieldEnemyManager.shield.transform.parent = null;
        shieldEnemyManager.shield.AddComponent<Rigidbody>();
        shieldEnemyManager.shield.AddComponent<BoxCollider>();

        shieldUnparented = true;

    }

    protected override void HandleAttacks()
    {

        base.HandleAttacks();
    }

    protected override void HandleMovements()
    {
        base.HandleMovements();
    }
}
