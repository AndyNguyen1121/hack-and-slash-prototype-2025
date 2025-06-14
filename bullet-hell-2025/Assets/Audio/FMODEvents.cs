using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("PlayerSounds")]
    [field: SerializeField] public EventReference footSteps {  get; private set; } 

    public static FMODEvents instance;
    private void Awake()
    {
       if (instance == null) 
            instance = this;
    }
}
