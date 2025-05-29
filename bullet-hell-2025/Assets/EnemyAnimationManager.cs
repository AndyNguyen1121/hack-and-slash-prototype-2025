using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationManager : MonoBehaviour
{
    private EnemyManager enemyManager;

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;

    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void OnAnimatorMove()
    {
        Vector3 deltaPosition = enemyManager.animator.deltaPosition;
        Vector3 rootPosition = enemyManager.animator.rootPosition;
        rootPosition.y = enemyManager.agent.nextPosition.y;

        deltaPosition.x *= enemyManager.moveSpeed;
        deltaPosition.z *= enemyManager.moveSpeed;

        rootPosition.x += deltaPosition.x;
        rootPosition.z += deltaPosition.z;

        transform.position = rootPosition;
        enemyManager.agent.nextPosition = rootPosition;
        
    }

    public void SetMovementParameters(float horizontal, float vertical)
    {
        Vector3 worldDeltaPosition = enemyManager.agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0f;
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        velocity = smoothDeltaPosition / Time.deltaTime;

        if (enemyManager.agent.remainingDistance <= enemyManager.agent.stoppingDistance)
        {
            velocity = Vector2.Lerp(Vector2.zero, velocity, enemyManager.agent.remainingDistance / enemyManager.agent.stoppingDistance);
        }

        bool shouldMove = velocity.magnitude > 0.5f && enemyManager.agent.remainingDistance > enemyManager.agent.stoppingDistance;

        enemyManager.animator.SetBool("isMoving", shouldMove);
        enemyManager.animator.SetFloat("horizontal", horizontal, 1.2f, Time.deltaTime * 6f);
        enemyManager.animator.SetFloat("vertical", vertical, 1.2f, Time.deltaTime * 6f);

        float deltaMagnitude = worldDeltaPosition.magnitude;

        if (deltaMagnitude > enemyManager.agent.radius / 2f)
        {
            transform.position = Vector3.Lerp(enemyManager.animator.rootPosition,
                                              enemyManager.agent.nextPosition,
                                              smooth);
        }
    }
}
