using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldEnemySpawnerManager : MonoBehaviour
{
    public GameObject testObject;
    public static WorldEnemySpawnerManager Instance { get; private set; }

    public List<Collider> spawnLocations = new();

    [SerializeField]
    private List<EnemyManager> allEnemies = new();

    [SerializeField]
    private List<EnemyManager> attackSignalList = new();

    [SerializeField]
    private List<EnemyManager> currentlyAttackingEnemies = new();

    public int maxAttackingEnemies = 2;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        StartCoroutine(TestSpawn());
    }
    // Update is called once per frame
    void Update()
    {
        TryActivateNextAttacker();
    }

    public void RegisterEnemy(EnemyManager enemy)
    {
        if (!allEnemies.Contains(enemy))
        {
            allEnemies.Add(enemy);
            enemy.SendAttackSignal += AddAttackSignal;
        }
    }

    public void UnregisterEnemy(EnemyManager enemy)
    {
        if (allEnemies.Contains(enemy))
        {
            allEnemies.Remove(enemy);
            enemy.SendAttackSignal -= AddAttackSignal;
            RemoveAttackSignal(enemy);
            RemoveFromAttackingPool(enemy);

            if (currentlyAttackingEnemies.Contains(enemy))
            {
                currentlyAttackingEnemies.Remove(enemy);
            }
        }
    }

    private void AddAttackSignal(EnemyManager enemy)
    {
        if (!attackSignalList.Contains(enemy) && !currentlyAttackingEnemies.Contains(enemy))
        {
            attackSignalList.Add(enemy);
            //TryActivateNextAttacker();
        }
    }

    public void RemoveAttackSignal(EnemyManager enemy)
    {
        if (attackSignalList.Contains(enemy))
        {
            attackSignalList.Remove(enemy);
            //TryActivateNextAttacker();
        }
    }

    private void TryActivateNextAttacker()
    {
        while (currentlyAttackingEnemies.Count < maxAttackingEnemies && attackSignalList.Count > 0)
        {
            EnemyManager closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (EnemyManager enemy in attackSignalList)
            {
                float distance = Vector3.Distance(PlayerManager.instance.transform.position, enemy.transform.position);

                if (distance < closestDistance && !currentlyAttackingEnemies.Contains(enemy)) 
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }

            if (closestEnemy != null)
            {
                closestEnemy.canAttack = true;
                currentlyAttackingEnemies.Add(closestEnemy);
            }

           /* var enemy = attackSignalList.Dequeue();

            if (allEnemies.Contains(enemy))
            {
                enemy.canAttack = true;
                currentlyAttackingEnemies.Add(enemy);
            }*/
        }
    }

    public void RemoveFromAttackingPool(EnemyManager enemy)
    {
        if (currentlyAttackingEnemies.Contains(enemy))
        {
            currentlyAttackingEnemies.Remove(enemy);
        }
    }

    public Vector3 GetColliderPosition()
    {
        int randomArea = Random.Range(0, spawnLocations.Count);

        Collider collider = spawnLocations[randomArea];
        Transform areaTransform = collider.transform;
        Vector3 cubeCenter = areaTransform.position;
        Vector3 cubeSize = collider.bounds.size;

        Vector3 randomPos = cubeCenter + new Vector3(
             Random.Range(-cubeSize.x / 2f, cubeSize.x / 2f),
             Random.Range(-cubeSize.y / 2f, cubeSize.y / 2f),
             Random.Range(-cubeSize.z / 2f, cubeSize.z / 2f));

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 2f, NavMesh.AllAreas))
        {
            randomPos = hit.position;
        }
        return randomPos;
    }

    public IEnumerator TestSpawn()
    {
        int count = 0;
        while (count < 8)
        {
            count++;
            Instantiate(testObject, GetColliderPosition(), Quaternion.identity);
            yield return new WaitForSeconds(1);
        }
    }
}
