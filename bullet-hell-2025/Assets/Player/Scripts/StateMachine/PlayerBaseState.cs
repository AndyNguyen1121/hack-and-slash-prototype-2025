using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public abstract class PlayerBaseState
{
    protected bool isRootState = false;
    protected PlayerStateMachine ctx;
    protected PlayerStateFactory factory;
    protected PlayerBaseState currentSuperState;
    protected PlayerBaseState currentSubState;

    public PlayerBaseState(PlayerStateMachine ctx, PlayerStateFactory factory)
    {
        this.ctx = ctx; 
        this.factory = factory;
    }
    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates() 
    {
        OnStateUpdate();

        if (currentSubState != null)
        {
            currentSubState.UpdateStates();
        }
    }
    void SwitchState(PlayerBaseState newState) 
    {
        OnStateExit();
        newState.OnStateEnter();

        if (isRootState)
        {
            ctx.currentState = newState;
        }
        else if (currentSuperState != null)
        {
            currentSuperState.SetSubState(newState);
        }
    }
    void SetSuperState(PlayerBaseState newSuperState)
    { 
        currentSuperState = newSuperState;
    }
    void SetSubState(PlayerBaseState newSubState)
    {
        currentSubState = newSubState;
        newSubState.SetSuperState(this);
    } 


}
