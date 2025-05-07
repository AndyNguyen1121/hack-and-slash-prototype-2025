using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack3 : PlayerCombatBaseState
{
    public PlayerAttack3(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        playerManager.playerAnimationManager.PlayActionAnimation("Attack3", true, true, true, false, false, true);
    }

    public override void UpdateState()
    {
        if (!playerManager.isPerformingAction && PlayerInputManager.instance.attackPressed)
        {
            stateMachine.SwitchState(factory.Attack4());
        }
    }

    public override void ExitState()
    {

    }
}
