using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hitstop Profile", menuName ="HitStop/New Hitstop Profile")]
public class HitstopBaseScript : ScriptableObject
{
    public float hitStopDuration;

    [Range(0f, 1f)]
    public float hitStopDelayPercentage;
}
