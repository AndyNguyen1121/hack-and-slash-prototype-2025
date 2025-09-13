using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldEnemyManager : MonoBehaviour
{
    public GameObject testObject;
    public static WorldEnemyManager Instance { get; private set; }

    public List<Collider> spawnLocations = new();

    [SerializeField]
    private List<EnemyManager> allEnemies = new();

    [SerializeField]
    private List<EnemyManager> meleeAttackSignalList = new();

    [SerializeField]
    private List<EnemyManager> rangedAttackSignalList = new();

    [SerializeField]
    private List<EnemyManager> currentlyAttackingMeleeEnemies = new();

    [SerializeField]
    private List<EnemyManager> currentlyAttackingRangedEnemies = new();

    public int maxAttackingMeleeEnemies = 2;
    public int maxAttackingRangedEnemies = 2;

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
       // StartCoroutine(TestSpawn());
    }
    // Update is called once per frame
    void Update()
    {
        TryActivateNextAttacker(ref currentlyAttackingMeleeEnemies, meleeAttackSignalList, maxAttackingMeleeEnemies);
        TryActivateNextAttacker(ref currentlyAttackingRangedEnemies, rangedAttackSignalList, maxAttackingRangedEnemies);
    }

    public void RegisterEnemy(EnemyManager enemy)
    {
        if (!allEnemies.Contains(enemy))
        {
            allEnemies.Add(enemy);

            if (enemy.enemyClassification == EnemyClassification.Melee)
            {
                enemy.SendAttackSignal += AddMeleeAttackSignal;
            }
            else if (enemy.enemyClassification == EnemyClassification.Ranged) 
            {
                enemy.SendAttackSignal += AddRangedAttackSignal;
            }
        }
    }

    public void UnregisterEnemy(EnemyManager enemy)
    {
        if (allEnemies.Contains(enemy))
        {
            allEnemies.Remove(enemy);
            enemy.ClearAllSubscribers();
            RemoveAttackSignal(enemy);
            RemoveFromAttackingPool(enemy);

            if (currentlyAttackingMeleeEnemies.Contains(enemy))
            {
                currentlyAttackingMeleeEnemies.Remove(enemy);
            }
            else if (currentlyAttackingRangedEnemies.Contains(enemy))
            {
                currentlyAttackingRangedEnemies.Remove(enemy);
            }
        }
    }

    private void AddMeleeAttackSignal(EnemyManager enemy)
    {
        if (!meleeAttackSignalList.Contains(enemy) 
            && !currentlyAttackingMeleeEnemies.Contains(enemy))
        {
            meleeAttackSignalList.Add(enemy);
        }
    }

    private void AddRangedAttackSignal(EnemyManager enemy)
    {
        if (!rangedAttackSignalList.Contains(enemy) 
            && !currentlyAttackingRangedEnemies.Contains(enemy))
        {
            rangedAttackSignalList.Add(enemy);
        }
    }

    public void RemoveAttackSignal(EnemyManager enemy)
    {
        if (enemy.enemyClassification == EnemyClassification.Melee 
            && meleeAttackSignalList.Contains(enemy))
        {
            meleeAttackSignalList.Remove(enemy);
        }
        else if (enemy.enemyClassification == EnemyClassification.Ranged 
            && rangedAttackSignalList.Contains(enemy))
        {
            rangedAttackSignalList.Remove(enemy);
        }
    }

    private void TryActivateNextAttacker(ref List<EnemyManager> currentAttackingEnemyList, List<EnemyManager> signalList, int maxActiveEnemies)
    {
        if (currentAttackingEnemyList.Count < maxActiveEnemies 
            && signalList.Count > 0 && allEnemies.Count > 0)
        {
            EnemyManager closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (EnemyManager enemy in signalList)
            {
                float distance = Vector3.Distance(PlayerManager.instance.transform.position, enemy.transform.position);

                if (distance < closestDistance && !currentAttackingEnemyList.Contains(enemy)) 
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                  
                }
            }

            if (closestEnemy != null)
            {
                RemoveAttackSignal(closestEnemy);
            }

            if (allEnemies.Contains(closestEnemy))
            {
                closestEnemy.canAttack = true;
                currentAttackingEnemyList.Add(closestEnemy);
            }
        }
    }

    public void RemoveFromAttackingPool(EnemyManager enemy)
    {
        if (enemy.enemyClassification == EnemyClassification.Melee 
            && currentlyAttackingMeleeEnemies.Contains(enemy))
        {
            currentlyAttackingMeleeEnemies.Remove(enemy);
        }
        else if (enemy.enemyClassification == EnemyClassification.Ranged 
            && currentlyAttackingRangedEnemies.Contains(enemy))
        {
            currentlyAttackingRangedEnemies.Remove(enemy);
        }
    }

   
}
