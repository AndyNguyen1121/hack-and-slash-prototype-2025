using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Experimental.AI;

public class WaveManager : MonoBehaviour
{
    public enum EnemyType
    { 
        Normal,
        Shield,
        Gun
    }

    public const int numOfPresetWaves = 15;
    public List<WaveSettings> waveSettings = new();

    private Dictionary<EnemyType, int> enemiesToSpawn = new();
    [SerializeField]
    private GameObject normalEnemy;

    [SerializeField]
    private GameObject shieldEnemy;

    [SerializeField]
    private GameObject gunEnemy;

    [SerializeField]
    private Collider spawnCollider;

    [SerializeField]
    private float spawnRate;

    [SerializeField]
    private float minimumDistanceFromPlayer = 3f;

    [Header("Wave Attributes")]
    public int waveCount;
    public float restDuration;
    public float healthMultiplier = 1;
    public float damageMultipler = 1;
    private float maxHealthMultiplier = 4f;
    private float maxDamageMultipler = 3f;

    [Header("Wave Information")]
    public int maxNumEnemies;
    public int currentEnemyCount;
    public int numEnemiesSpawnDuringThisWave;
    public bool waveSpawnIsComplete;
    public TextMeshProUGUI waveText;

    private Task currentWave;
    private int currentWaveReference;

    public UnityEvent OnEnemyKilled;
    private void Start()
    {
        waveCount = 1;
        OnEnemyKilled.AddListener(CheckIfWaveIsOver);
        InitializeNewWave();

        waveText.text = "wave 1";
    }

    public void CheckIfWaveIsOver()
    {
        if (waveSpawnIsComplete && currentEnemyCount == 0)
        {
            waveCount++;
            InitializeNewWave();
        }
    }

    public void InitializeNewWave()
    {
        // default value if waveCount does not exceed number of preset waves
        currentWaveReference = waveCount - 1;

        // Adjust wave difficulty if reached max number of preset waves, set wave to numOfPresetWaves
        if (waveCount > numOfPresetWaves)
        {
            currentWaveReference = numOfPresetWaves - 1;
            spawnRate = waveSettings[currentWaveReference].spawnRate;
            healthMultiplier = Mathf.Min(waveSettings[currentWaveReference].healthMultiplier + ((waveCount - numOfPresetWaves) * 0.2f), maxHealthMultiplier);
            damageMultipler = Mathf.Min(waveSettings[currentWaveReference].damageMultiplier + ((waveCount - numOfPresetWaves * 0.2f)), maxDamageMultipler);
        }
        else
        {
            spawnRate = waveSettings[currentWaveReference].spawnRate;
            healthMultiplier = waveSettings[currentWaveReference].healthMultiplier;
            damageMultipler = waveSettings[currentWaveReference].damageMultiplier;
        }

        enemiesToSpawn[EnemyType.Normal] = waveSettings[currentWaveReference].normalEnemyCount;
        enemiesToSpawn[EnemyType.Shield] = waveSettings[currentWaveReference].shieldEnemyCount;
        enemiesToSpawn[EnemyType.Gun] = waveSettings[currentWaveReference].gunEnemyCount;

        currentWave = BeginNewWave();

    }

    public void DecreaseCurrentEnemyCount()
    {
        --currentEnemyCount;
        OnEnemyKilled.Invoke();
    }

    async Task BeginNewWave()
    {
        waveSpawnIsComplete = false;
        numEnemiesSpawnDuringThisWave = 0;
        waveText.text = "wave " + waveCount;
        Debug.Log(waveCount);
        waveText.gameObject.SetActive(false);
        waveText.gameObject.SetActive(true);

        maxNumEnemies = enemiesToSpawn[EnemyType.Normal] + enemiesToSpawn[EnemyType.Shield] + enemiesToSpawn[EnemyType.Gun];

        while (numEnemiesSpawnDuringThisWave < maxNumEnemies)
        {
            // check which types of enemies can be spawned
            List<EnemyType> enemyTypesToSelect = new();

            if (enemiesToSpawn[EnemyType.Normal] > 0)
                enemyTypesToSelect.Add(EnemyType.Normal);

            if (enemiesToSpawn[EnemyType.Shield] > 0)
                enemyTypesToSelect.Add (EnemyType.Shield);

            if (enemiesToSpawn[EnemyType.Gun] > 0)
                enemyTypesToSelect.Add(EnemyType.Gun);

            EnemyType randomEnemy = enemyTypesToSelect[Random.Range(0, enemyTypesToSelect.Count)];


            Vector3 spawnPosition = GetColliderPosition();
            GameObject enemy = null;

            switch (randomEnemy)
            {
                case EnemyType.Normal:
                    enemy = Instantiate(normalEnemy, spawnPosition, Quaternion.identity);
                    --enemiesToSpawn[EnemyType.Normal];
                    break;

                case EnemyType.Shield:
                    enemy = Instantiate(shieldEnemy, spawnPosition, Quaternion.identity);
                    --enemiesToSpawn[EnemyType.Shield];
                    break;

                case EnemyType.Gun:
                    --enemiesToSpawn[EnemyType.Gun];
                    enemy = Instantiate(gunEnemy, spawnPosition, Quaternion.identity);
                    break;
            };

            // set health multiplier
            IDamageable healthScript = null;    
            if (enemy.TryGetComponent<IDamageable>(out healthScript))
            {
                healthScript.MaxHealth *= healthMultiplier;
            }

            // set damage multipler
            EnemyCombatManager damageScript = null;
            if (enemy.TryGetComponent<EnemyCombatManager>(out damageScript))
            {
                damageScript.damage *= damageMultipler;
            }

            numEnemiesSpawnDuringThisWave++;
            OnEnemyKilled onEnemyKilled = enemy.AddComponent<OnEnemyKilled>();
            onEnemyKilled.waveManager = this;
            ++currentEnemyCount;

            await Task.Delay((int)spawnRate * 1000);
        }

        waveSpawnIsComplete = true;
    }

    public Vector3 GetColliderPosition()
    {

        Transform areaTransform = spawnCollider.transform;
        Vector3 cubeCenter = spawnCollider.bounds.center;
        Vector3 cubeSize = spawnCollider.bounds.size;

        Vector3 randomPos = cubeCenter + new Vector3(
             Random.Range(-cubeSize.x / 2f, cubeSize.x / 2f),
             0,
             Random.Range(-cubeSize.z / 2f, cubeSize.z / 2f));

        // Adjust position if too close to player
        if (Vector3.Distance(PlayerManager.instance.transform.position, randomPos) < minimumDistanceFromPlayer)
        {
            Vector3 dirToAdjustSpawnLocation = (randomPos - PlayerManager.instance.transform.position).normalized;
            dirToAdjustSpawnLocation.y = 0;
            randomPos = PlayerManager.instance.transform.position + (dirToAdjustSpawnLocation * minimumDistanceFromPlayer);
            randomPos.y = spawnCollider.transform.position.y;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 2f, NavMesh.AllAreas))
        {
            randomPos = hit.position;
        }
        return randomPos;
    }

   
}
