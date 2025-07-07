using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("PlayerSounds")]
    [field: SerializeField] public EventReference footSteps { get; private set; }
    [field: SerializeField] public EventReference swordSlash { get; private set; }
    [field: SerializeField] public EventReference swordHit { get; private set; }
    [field: SerializeField] public EventReference swordParry { get; private set; }
    [field: SerializeField] public EventReference grapple { get; private set; }
    public static FMODEvents instance;
    private void Awake()
    {
       if (instance == null) 
            instance = this;
    }
}
