using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationManager : MonoBehaviour
{
    public EnemyManager enemyManager;
    public Animator animator;

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;
    private Vector3 lastPosition;


    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        animator = enemyManager.animator;
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void OnAnimatorMove()
    {
        if (animator == null)
            return; 
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

        Vector2 movementDir = new Vector2(horizontal, vertical);
        enemyManager.animator.SetBool("isMoving", movementDir != Vector2.zero);
        enemyManager.animator.SetFloat("horizontal", horizontal, 0.6f, Time.deltaTime * 6f);
        enemyManager.animator.SetFloat("vertical", vertical, 0.6f, Time.deltaTime * 6f);

        float deltaMagnitude = worldDeltaPosition.magnitude;

        if (deltaMagnitude > enemyManager.agent.radius / 2f)
        {
            transform.position = Vector3.Lerp(enemyManager.animator.rootPosition,
                                              enemyManager.agent.nextPosition,
                                              smooth);
        }

        lastPosition = transform.position;
    }

    public void PlayActionAnimation(string animationName, bool rootMotion = true, bool isPerformingAction = true, float normalizedTime = 0.1f)
    {
        animator.applyRootMotion = rootMotion;
        enemyManager.isPerformingAction = isPerformingAction;

        if (normalizedTime > 0f)
        {
            animator.CrossFade(animationName, normalizedTime);
        }
        else
        {
            animator.Play(animationName, 0, 0);
        }
    }
}
