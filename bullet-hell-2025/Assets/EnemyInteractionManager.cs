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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpToHeightInTime(2);
        }

        CheckGroundedState();

        timeOnGround += Time.deltaTime;

        if (timeOnGround > minTimeOnGround && inKnockUpAnimation && isGrounded)
        {
            animator.CrossFade("KnockupEnd", 0.1f);
            inKnockUpAnimation = false;
        }
    }

    private void CheckGroundedState()
    {
        isGrounded = Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckRadius, whatIsGround);
    }

    public void JumpToHeightInTime(float height)
    {
        if (height == 0)
            return;

        //enemyManager.agent.enabled = false;
        //if (!inKnockUpAnimation)
            animator.Play("Knockup", 0, 0f);

        inKnockUpAnimation = true;
        rb.velocity = Vector3.zero;

        rb.AddForce(Vector3.up * height, ForceMode.Impulse);

        timeOnGround = 0f;


    }

    public void SlamDown(float slamForce)
    {
        Vector3 velocity = rb.velocity;
        velocity.y = 0f;
        rb.velocity = velocity;

        rb.AddForce(Vector3.down * slamForce, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }
}
