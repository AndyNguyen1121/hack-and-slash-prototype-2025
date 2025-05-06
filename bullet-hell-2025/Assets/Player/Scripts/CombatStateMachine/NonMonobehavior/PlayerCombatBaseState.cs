using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCombatBaseState
{
    protected PlayerCombatStateMachine stateMachine;
    protected PlayerCombatStateFactory factory;
    protected PlayerManager playerManager;

    public PlayerCombatBaseState(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory)
    {
        this.stateMachine = stateMachine;  
        this.factory = factory;
        playerManager = PlayerManager.instance;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
