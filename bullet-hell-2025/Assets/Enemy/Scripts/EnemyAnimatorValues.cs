using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorValues : StateMachineBehaviour
{
    public bool isStunned;
    public bool isPerformingAction;
    public bool enableAgent;
    public bool disableWeaponCollider;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyManager enemyManager = animator.GetComponent<EnemyManager>();

        if (enemyManager != null)
        {
            enemyManager.enemyBehavior.isStunned = isStunned;
            enemyManager.isPerformingAction = isPerformingAction;

            if (!enableAgent)
            {
                enemyManager.agent.enabled = enableAgent;
            }
            else
            {
                enemyManager.agent.enabled = enableAgent;
                enemyManager.enemyBehavior.canMove = true;
            }
            
            if (disableWeaponCollider)
            {
                enemyManager.enemyCombatManager.CloseWeaponCollider();
            }
        }
    }

    
}
