using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{

    public PlayerGroundedState(PlayerStateMachine context, PlayerStateFactory playerStateFactory) : base(context, playerStateFactory) 
    {
        isRootState = true;
        InitializeSubState();
    }

    public override void OnStateEnter() 
    {
        
    }

    public override void OnStateUpdate() 
    {
        CheckSwitchStates();
    }

    public override void OnStateExit() { }

    public override void InitializeSubState() { }

    public override void CheckSwitchStates() { }
}
