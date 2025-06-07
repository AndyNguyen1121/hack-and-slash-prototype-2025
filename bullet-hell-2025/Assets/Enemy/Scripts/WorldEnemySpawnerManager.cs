using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEnemySpawnerManager : MonoBehaviour
{
    public static WorldEnemySpawnerManager Instance { get; private set; }

    [SerializeField]
    private List<EnemyManager> allEnemies = new();

    [SerializeField]
    private Queue<EnemyManager> attackSignalQueue = new();

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
            enemy.SendAttackSignal += HandleAttackSignal;
        }
    }

    private void HandleAttackSignal(EnemyManager enemy)
    {
        if (!attackSignalQueue.Contains(enemy) && !currentlyAttackingEnemies.Contains(enemy))
        {
            attackSignalQueue.Enqueue(enemy);
            TryActivateNextAttacker();
        }
    }

    private void TryActivateNextAttacker()
    {
        while (currentlyAttackingEnemies.Count < maxAttackingEnemies && attackSignalQueue.Count > 0)
        {
            var enemy = attackSignalQueue.Dequeue();
            enemy.canAttack = true;
            currentlyAttackingEnemies.Add(enemy);
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
