using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemyBehavior : EnemyBehavior
{
    public GameObject shield;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
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
        shield.transform.parent = null;
        shield.AddComponent<Rigidbody>();
        shield.AddComponent<BoxCollider>();

        shieldUnparented = true;
    }
}
