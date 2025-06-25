using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEnemySpawnerManager : MonoBehaviour
{
    public static WorldEnemySpawnerManager Instance { get; private set; }

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
}
