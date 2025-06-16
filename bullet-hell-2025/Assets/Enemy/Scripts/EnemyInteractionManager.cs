using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInteractionManager : MonoBehaviour
{
    public Rigidbody rb;
    public EnemyManager enemyManager;
    public Animator animator;

    public bool inKnockUpAnimation = false;

    [Header("Ground Check")]
    public bool isGrounded = true;
    public float groundCheckRadius;
    public Vector3 groundCheckOffset;
    public LayerMask whatIsGround;
    public float minTimeOnGround = 0.1f;
    public float timeOnGround;
    public virtual void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public virtual void Update()
    {

        CheckGroundedState();

        timeOnGround += Time.deltaTime;

        
    }

    private void CheckGroundedState()
    {
        if (!enemyManager.isAlive)
            return;

        isGrounded = Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckRadius, whatIsGround);
        if (timeOnGround > minTimeOnGround && inKnockUpAnimation && isGrounded)
        {
            animator.CrossFade("KnockupEnd", 0.1f);
            inKnockUpAnimation = false;
            rb.isKinematic = true;
        }
    }

    public virtual void JumpToHeightInTime(float height)
    {
        Debug.Log("hit");
        timeOnGround = 0f;
        if (height == 0 || !enemyManager.isAlive)
            return;

        if (!inKnockUpAnimation)
            animator.Play("KnockUp", 0, 0f);
        else
            animator.Play("KnockUpRestart", 0, 0f);

        inKnockUpAnimation = true;
        
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float mass = rb.mass;

        float velocity = Mathf.Sqrt(2f * gravity * Mathf.Abs(height));

        float impulseForce = mass * velocity;

        Vector3 direction = height > 0 ? Vector3.up : Vector3.down;

        rb.AddForce(direction * impulseForce, ForceMode.Impulse);


    }

    public virtual void KnockBackRigidbody(float force, Vector3 directionOfImpact)
    {
        timeOnGround = 0f;

        if (force == 0 || !enemyManager.isAlive)
            return;

        // Play the knock-up animation
        if (!inKnockUpAnimation)
            animator.Play("KnockUp", 0, 0f);
        else
            animator.Play("KnockUpRestart", 0, 0f);

        inKnockUpAnimation = true;

        rb.isKinematic = false;
        rb.velocity = Vector3.zero;

        // Normalize direction of impact
        Vector3 dirFromPlayer = transform.position - directionOfImpact;
        Vector3 flatDirection = dirFromPlayer;
        flatDirection.y = 0f;

        if (flatDirection == Vector3.zero)
            flatDirection = transform.forward;

        flatDirection.Normalize();

        // Add upward component at 45°
        float angleInDegrees = 30f;
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        Vector3 knockbackDirection =
            Mathf.Cos(angleInRadians) * flatDirection +
            Mathf.Sin(angleInRadians) * Vector3.up;

        // Calculate impulse
        float gravity = Mathf.Abs(Physics.gravity.y);
        float mass = rb.mass;
        float velocity = Mathf.Sqrt(2f * gravity * Mathf.Abs(force));
        float impulseForce = mass * velocity;

        // Apply force
        rb.AddForce(knockbackDirection * impulseForce, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }
}
