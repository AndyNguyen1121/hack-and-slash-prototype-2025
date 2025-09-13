using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveSetting", menuName = "WaveSettings")]
public class WaveSettings : ScriptableObject
{
    public int normalEnemyCount;
    public int shieldEnemyCount;
    public int gunEnemyCount;
    

    [Space(10)]
    public float spawnRate;
    public float damageMultiplier = 1;
    public float healthMultiplier = 1;
}
