using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatStateFactory 
{
    public PlayerCombatStateMachine stateMachine;

    //States
    private PlayerIdle idle;
    private PlayerAttack1 attack1;
    private PlayerAttack2 attack2;
    private PlayerAttack3 attack3;  
    private PlayerAttack4 attack4;

    public PlayerCombatStateFactory(PlayerCombatStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;

        attack1 = new PlayerAttack1(stateMachine, this);
        attack2 = new PlayerAttack2(stateMachine, this);
        attack3 = new PlayerAttack3(stateMachine, this);
        attack4 = new PlayerAttack4(stateMachine, this);
        idle = new PlayerIdle(stateMachine, this);
    }

    public PlayerCombatBaseState Idle()
    {
        return idle;
    }
    public PlayerCombatBaseState Attack1()
    {
        return attack1;
    }

    public PlayerCombatBaseState Attack2()
    {
        return attack2;
    }

    public PlayerCombatBaseState Attack3()
    {
        return attack3;
    }

    public PlayerCombatBaseState Attack4()
    {
        return attack4;
    }
}
