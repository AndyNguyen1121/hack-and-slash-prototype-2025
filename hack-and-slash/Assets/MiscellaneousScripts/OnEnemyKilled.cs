using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnemyKilled : MonoBehaviour
{
    public WaveManager waveManager;

    private void OnDestroy()
    {
        waveManager.DecreaseCurrentEnemyCount();
    }
}
