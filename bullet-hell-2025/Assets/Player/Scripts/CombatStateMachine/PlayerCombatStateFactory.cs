using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;


public enum PlayerCombatState
{
    Idle,
    Attack1,
    Attack2,
    Attack3,
    Attack4,
    Uppercut,

    JumpAttack1,
    JumpAttack2,
    JumpAttack3,
    JumpAttack4,
    JumpAttack5,
    JumpAttack6,
    Helmbreak
}

public class StateInfo
{
    public PlayerCombatBaseState combatState;
    public CombatScriptableObj stateObj;

    public StateInfo(PlayerCombatBaseState combatBaseState, CombatScriptableObj stateInformation)
    {
        this.combatState = combatBaseState;
        this.stateObj = stateInformation;
    }
}

public class PlayerCombatStateFactory 
{
    public PlayerCombatStateMachine stateMachine;

    //States
    private PlayerIdle idle;
    private PlayerAttack1 attack1;
    private PlayerAttack2 attack2;
    private PlayerAttack3 attack3;  
    private PlayerAttack4 attack4;
    private PlayerUppercut uppercut;

    private PlayerJumpAttack1 jumpAttack1;
    private PlayerJumpAttack2 jumpAttack2;
    private PlayerJumpAttack3 jumpAttack3;
    private PlayerJumpAttack4 jumpAttack4;
    private PlayerJumpAttack5 jumpAttack5;
    private PlayerJumpAttack6 jumpAttack6;
    private PlayerHelmbreak helmbreak;
    

    public Dictionary<PlayerCombatState, StateInfo> stateList;
    
    public PlayerCombatStateFactory(PlayerCombatStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;

        idle = new PlayerIdle(stateMachine, this);
        attack1 = new PlayerAttack1(stateMachine, this);
        attack2 = new PlayerAttack2(stateMachine, this);
        attack3 = new PlayerAttack3(stateMachine, this);
        attack4 = new PlayerAttack4(stateMachine, this);
        uppercut = new PlayerUppercut(stateMachine, this);

        jumpAttack1 = new PlayerJumpAttack1(stateMachine, this);
        jumpAttack2 = new PlayerJumpAttack2(stateMachine, this);
        jumpAttack3 = new PlayerJumpAttack3(stateMachine, this);
        jumpAttack4 = new PlayerJumpAttack4(stateMachine, this);
        jumpAttack5 = new PlayerJumpAttack5(stateMachine, this);
        jumpAttack6 = new PlayerJumpAttack6(stateMachine, this);
        helmbreak = new PlayerHelmbreak(stateMachine, this);


        stateList = new Dictionary<PlayerCombatState, StateInfo> {
            { PlayerCombatState.Idle, new StateInfo(idle, stateMachine.idleInfo)},
            { PlayerCombatState.Attack1, new StateInfo(attack1, stateMachine.attack1Info) },
            { PlayerCombatState.Attack2, new StateInfo(attack2, stateMachine.attack2Info) },
            { PlayerCombatState.Attack3, new StateInfo(attack3, stateMachine.attack3Info) },
            { PlayerCombatState.Attack4, new StateInfo(attack4, stateMachine.attack4Info) },
            { PlayerCombatState.Uppercut, new StateInfo(uppercut, stateMachine.uppercutInfo) },

            { PlayerCombatState.JumpAttack1, new StateInfo(jumpAttack1, stateMachine.jumpAttack1Info) },
            { PlayerCombatState.JumpAttack2, new StateInfo(jumpAttack2, stateMachine.jumpAttack2Info) },
            { PlayerCombatState.JumpAttack3, new StateInfo(jumpAttack3, stateMachine.jumpAttack3Info) },
            { PlayerCombatState.JumpAttack4, new StateInfo(jumpAttack4, stateMachine.jumpAttack4Info) },
            { PlayerCombatState.JumpAttack5, new StateInfo(jumpAttack5, stateMachine.jumpAttack5Info) },
            { PlayerCombatState.JumpAttack6, new StateInfo(jumpAttack6, stateMachine.jumpAttack6Info) },
            { PlayerCombatState.Helmbreak, new StateInfo(helmbreak, stateMachine.helmBreakInfo) }

            };
    }
}

