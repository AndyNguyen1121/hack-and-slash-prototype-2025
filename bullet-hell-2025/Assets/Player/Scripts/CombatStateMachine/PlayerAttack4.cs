using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack4 : PlayerCombatBaseState
{
    public PlayerAttack4(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        playerManager.playerAnimationManager.PlayActionAnimation("Attack4", true, true, true, false, false, true);
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }
}
