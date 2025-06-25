using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.OpenVR;
using UnityEngine;

public class ShieldEnemyInteractionManager : EnemyInteractionManager
{
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

    }

    public override void JumpToHeightInTime(float height)
    {
        if (enemyManager is ShieldEnemyManager shieldManager && shieldManager.isGuarding)
        {
            Debug.Log("Fuckin loser");
            return;
        }
        base.JumpToHeightInTime(height);
    }

    public override void KnockBackRigidbody(float force, Vector3 directionOfImpact)
    {
        if (enemyManager is ShieldEnemyManager shieldManager && shieldManager.isGuarding)
        {
            Debug.Log("Fuckin loser");
            return;
        }
        base.KnockBackRigidbody(force, directionOfImpact);
    }


}
