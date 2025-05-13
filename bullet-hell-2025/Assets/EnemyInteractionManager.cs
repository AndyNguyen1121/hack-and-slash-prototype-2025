using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInteractionManager : MonoBehaviour
{
    public Rigidbody rb;
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
            animator.CrossFade("KnockupEnd", 0.01f);
            inKnockUpAnimation = false;
        }
    }

    private void CheckGroundedState()
    {
        isGrounded = Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckRadius, whatIsGround);
    }

    public void JumpToHeightInTime(float height)
    {
        if (!inKnockUpAnimation)
            animator.Play("Knockup", 0, 0f);

        inKnockUpAnimation = true;
        float g = Mathf.Abs(Physics.gravity.y); 
        float t = 0.5f;
        float h = height;

        float requiredVelocity = (h + 0.5f * g * t * t) / t;
        float impulse = rb.mass * requiredVelocity;
        rb.AddForce(Vector3.up * impulse, ForceMode.Impulse);

        timeOnGround = 0f;


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }
}
