using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance;
    [field: Header("PlayerSounds")]
    [field: SerializeField] public EventReference footSteps { get; private set; }
    [field: SerializeField] public EventReference swordSlash { get; private set; }
    [field: SerializeField] public EventReference swordHit { get; private set; }
    [field: SerializeField] public EventReference swordParry { get; private set; }
    [field: SerializeField] public EventReference grapple { get; private set; }
    [field: SerializeField] public EventReference roll { get; private set; }

    [field: Space(10)]

    [field: Header("Enemy Sounds")]
    [field: SerializeField] public EventReference enemyGrowlAggressive { get; private set; }
    [field: SerializeField] public EventReference enemyDeath { get; private set; }
    [field: SerializeField] public EventReference enemyGrowlIdle { get; private set; }
    [field: SerializeField] public EventReference enemyAttack { get; private set; }
    [field: SerializeField] public EventReference enemyAxeHit { get; private set; }
    [field: SerializeField] public EventReference laserShoot { get; private set; }
    [field: SerializeField] public EventReference laserHit { get; private set; }
    [field: SerializeField] public EventReference shieldBreak { get; private set; }
    [field: SerializeField] public EventReference shieldHit { get; private set; }

    [field: Space(10)]

    [field: Header("UI Sounds")]
    [field: SerializeField] public EventReference buttonSwitch { get; private set; }
    [field: SerializeField] public EventReference buttonPress { get; private set; }
    [field: SerializeField] public EventReference menuClose { get; private set; }
    [field: SerializeField] public EventReference menuOpen { get; private set; }
    [field: SerializeField] public EventReference sliderChange { get; private set; }

    [field: Header("Door Sounds")]
    [field: SerializeField] public EventReference doorClose { get; private set; }
    [field: SerializeField] public EventReference doorOpen { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
       if (instance == null) 
            instance = this;
    }
}
