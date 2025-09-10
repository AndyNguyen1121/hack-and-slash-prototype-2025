using DG.Tweening;
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
    private bool ignoreGroundDetection = false;
    private Coroutine ignoreGroundCoroutine;

    // during certain actions, ignore ground detection
    public float ignoreGroundDetectionDuration = 0.05f;

    private Tween grappleTween;
    public virtual void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public virtual void Update()
    {
        CheckGroundedState();
    }

    private void CheckGroundedState()
    {
        if (!enemyManager.isAlive)
            return;
        
        if (ignoreGroundDetection)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckRadius, whatIsGround);
        if (isGrounded)
        {
            timeOnGround += Time.deltaTime;
        }
        else
        {
            timeOnGround = 0f;
        }
        //timeOnGround > minTimeOnGround && 
        if (inKnockUpAnimation && isGrounded)
        {
            animator.CrossFade("KnockupEnd", 0.15f);
            inKnockUpAnimation = false;
            rb.isKinematic = true;
            timeOnGround = 0f;
        }
    }

    private void IgnoreGroundDetection(float duration)
    {
        if (ignoreGroundCoroutine != null)
        {
            StopCoroutine(ignoreGroundCoroutine);
        }

        ignoreGroundCoroutine = StartCoroutine(IgnoreGroundDetectionCoroutine(duration));
    }
    private IEnumerator IgnoreGroundDetectionCoroutine(float time)
    {
        ignoreGroundDetection = true;
        yield return new WaitForSeconds(time);
        ignoreGroundDetection = false;
    }

    public virtual void JumpToHeightInTime(float height)
    {
        timeOnGround = 0f;
        IgnoreGroundDetection(ignoreGroundDetectionDuration);

        if (height == 0 || !enemyManager.isAlive)
            return;

        if (!inKnockUpAnimation)
            animator.Play("KnockUp", 0, 0f);
        else
            animator.Play("KnockUpRestart", 0, 0f);

        if (grappleTween != null)
            grappleTween.Kill();

        rb.isKinematic = false;
        rb.velocity = Vector3.zero;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float mass = rb.mass;
        float velocity = Mathf.Sqrt(2f * gravity * Mathf.Abs(height));
        float impulseForce = mass * velocity;
        Vector3 direction = height > 0 ? Vector3.up : Vector3.down;

        rb.AddForce(direction * impulseForce, ForceMode.Impulse);
        inKnockUpAnimation = true;
    }

    public virtual void KnockBackRigidbody(float force, Vector3 directionOfImpact)
    {
        timeOnGround = 0f;
        IgnoreGroundDetection(ignoreGroundDetectionDuration + 0.1f);

        if (force == 0 || !enemyManager.isAlive)
            return;

        // Play the knock-up animation
        if (!inKnockUpAnimation)
            animator.Play("KnockUp", 0, 0f);
        else
            animator.Play("KnockUpRestart", 0, 0f);

        if (grappleTween != null)
            grappleTween.Kill();

        inKnockUpAnimation = true;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        Vector3 dirFromPlayer = transform.position - directionOfImpact;
        Vector3 flatDirection = dirFromPlayer;
        flatDirection.y = 0f;

        if (flatDirection == Vector3.zero)
            flatDirection = transform.forward;

        flatDirection.Normalize();

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

    public virtual void Grapple(float stopDistance, float yOffset)
    {
        if (!enemyManager.canGrapple)
            return;
        
        enemyManager.enemyBehavior.isStunned = true;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;

        Vector3 dirFromPlayer = (transform.position - PlayerManager.instance.transform.position).normalized;
        dirFromPlayer.y = 0f;
        Vector3 desiredPosition = PlayerManager.instance.transform.position + (stopDistance * dirFromPlayer) + (Vector3.up * yOffset);

        if (grappleTween != null)
            grappleTween.Kill();

        transform.DORotateQuaternion(Quaternion.LookRotation(-dirFromPlayer), 0.25f);

        if (Mathf.Abs(desiredPosition.y - transform.position.y) > 0.25f) 
        {
            timeOnGround = 0f;
            IgnoreGroundDetection(ignoreGroundDetectionDuration);

            if (!inKnockUpAnimation)
                animator.Play("KnockUp", 0, 0f);
            else
                animator.Play("KnockUpRestart", 0, 0f);

            inKnockUpAnimation = true;
            rb.isKinematic = false;
        }
        else
        {
            enemyManager.TakeDamage(0.1f, transform.position, gameObject);
            
            if (inKnockUpAnimation && isGrounded)
            {
                inKnockUpAnimation = false;
            }

            desiredPosition.y = transform.position.y ;
        }

        grappleTween = rb.DOMove(desiredPosition, 0.25f).SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                if (!inKnockUpAnimation)
                    rb.isKinematic = true;
            });
           

        /*rb.velocity = CalculateParabolaVelocity(transform.position, PlayerManager.instance.transform.position, transform.position.y - PlayerManager.instance.transform.position.y);
        Debug.Log(CalculateParabolaVelocity(transform.position, PlayerManager.instance.transform.position, 3));*/
    }

    Vector3 CalculateParabolaVelocity(Vector3 start, Vector3 end, float height)
    {
        float gravity = Physics.gravity.y;
        gravity = Mathf.Abs(gravity); 

        Vector3 displacement = end - start;
        Vector3 displacementXZ = new Vector3(displacement.x, 0f, displacement.z);

        float timeToPeak = Mathf.Sqrt(2 * height / gravity);
        float totalTime = timeToPeak + Mathf.Sqrt(2 * Mathf.Max(0, height - displacement.y) / gravity);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / totalTime;

        return velocityXZ + velocityY;
    }

}
