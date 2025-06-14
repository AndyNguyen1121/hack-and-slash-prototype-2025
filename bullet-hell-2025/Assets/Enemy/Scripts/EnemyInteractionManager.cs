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
    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        CheckGroundedState();

        timeOnGround += Time.deltaTime;

        if (timeOnGround > minTimeOnGround && inKnockUpAnimation && isGrounded)
        {
            animator.CrossFade("KnockupEnd", 0.1f);
            inKnockUpAnimation = false;
            rb.isKinematic = true;
        }
    }

    private void CheckGroundedState()
    {
        isGrounded = Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckRadius, whatIsGround);
    }

    public void JumpToHeightInTime(float height)
    {
        Debug.Log("hit");
        timeOnGround = 0f;
        if (height == 0)
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

    public void KnockBackRigidbody(float force, Vector3 directionOfImpact)
    {
        if (!inKnockUpAnimation)
            animator.Play("KnockUp", 0, 0f);
        else
            animator.Play("KnockUpRestart", 0, 0f);

        directionOfImpact.Normalize();
        directionOfImpact.y = 0f;

        Vector3 newDir = Quaternion.AngleAxis(45f, transform.right) * directionOfImpact;
        inKnockUpAnimation = true;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;

        rb.AddForce(newDir * force, ForceMode.Impulse);

        timeOnGround = 0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }
}
