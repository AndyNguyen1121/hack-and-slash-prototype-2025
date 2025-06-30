using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class RobotBehavior : MonoBehaviour
{
    private RobotFollow robotFollow;
    private DG.Tweening.Sequence grapplingSequence;
    private Vector3 upDirection;
    public LineRenderer lineRenderer;
    public bool isGrappling;

    [Header("Grapple Attributes")]
    public float outDuration;
    public float inDuration;
    public int quality;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve affectCurve;
    public float damper;
    public float strength;
    public float velocity;
    private Spring spring;
    public float stopDistance;

    private void Awake()
    {
        spring = new Spring();
        spring.SetTarget(0);
    }
    // Start is called before the first frame update
    void Start()
    {
        robotFollow = GetComponent<RobotFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleGrapplingBehavior();
        
    }

    private void LateUpdate()
    {
        AnimateGrapple();
    }

    private Vector3 currentGrapplePos;
    void HandleGrapplingBehavior()
    {
        if (PlayerManager.instance.playerCameraManager.cursorPosition == null)
        {
            lineRenderer.enabled = false;
            grapplingSequence.Kill();
            isGrappling = false;
        }

        //lineRenderer.SetPosition(0, robotFollow.podObject.position);
        if (PlayerInputManager.instance.currentInputs.Contains(InputID.LockOn) && 
            PlayerInputManager.instance.currentInputs.Contains(InputID.Interact) && 
            PlayerManager.instance.playerCameraManager.cursorPosition != null && 
            !isGrappling)
        {
            lineRenderer.enabled = true;
            float elapsed = 0f;
            //Vector3 start = lineRenderer.GetPosition(1);

            if (grapplingSequence != null)
            {
                grapplingSequence.Kill();
            }

            grapplingSequence =
                DOTween.Sequence()
                .AppendCallback(() => isGrappling = true)
                .Append(

                    DOTween.To(() => 0f, x =>
                    {
                        elapsed = x;
                        Vector3 updatedEndPos = PlayerManager.instance.playerCameraManager.cursorPosition.position;
                        Vector3 updatedStartPos = robotFollow.podObject.position;
                        currentGrapplePos = Vector3.Lerp(updatedStartPos, updatedEndPos, x / outDuration);
                        upDirection = Quaternion.LookRotation((updatedEndPos - updatedStartPos).normalized) * Vector3.up;
                    }, outDuration, outDuration))

                 .AppendCallback(Grapple)
                 //.AppendCallback(PlayerManager.instance.IgnoreEnemyLayerCollision)
                 .Append(

                    DOTween.To(() => 0f, x =>
                    {
                        elapsed = x;
                        Vector3 updatedStartPos = PlayerManager.instance.playerCameraManager.cursorPosition.position;
                        Vector3 updatedEndPos = robotFollow.podObject.position;
                        currentGrapplePos = Vector3.Lerp(updatedStartPos, updatedEndPos, x / inDuration);
                        upDirection = Quaternion.LookRotation((updatedEndPos - updatedStartPos).normalized) * Vector3.up;
                    }, inDuration, inDuration))

                .AppendCallback(() => isGrappling = false);
               // .AppendInterval(0.2f)
                //.AppendCallback(PlayerManager.instance.AttemptToEnableEnemyCollision);

        }
    }

    void Grapple()
    {
        if (PlayerManager.instance.playerCameraManager.lockedOnEnemy != null)
        {
            PlayerManager.instance.playerCameraManager.lockedOnEnemy.enemyInteractionManager.Grapple(stopDistance);
        }
    }

    void AnimateGrapple()
    {
        if (!isGrappling)
        {
            currentGrapplePos = robotFollow.podObject.position;
            spring.Reset();
            if (lineRenderer.positionCount > 0)
            {
                lineRenderer.positionCount = 0;
            }

            return;
        }

        if (lineRenderer.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lineRenderer.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        /*for (var i = 0; i < quality + 1; i++)
        {
            var right = Quaternion.LookRotation((PlayerManager.instance.playerCameraManager.cursorPosition.position - robotFollow.podObject.position).normalized) * Vector3.right;


            var delta = i / (float)quality;
            var offset = upDirection * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta) +
                         right * waveHeight * Mathf.Cos(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta);

            lineRenderer.SetPosition(i, Vector3.Lerp(robotFollow.podObject.position, currentGrapplePos, delta) + offset);
        }*/

        for (int i = 0; i < quality + 1; ++i)
        {
            float delta = i / (float)quality;
            Vector3 offset = upDirection *
                waveHeight *
                Mathf.Sin(delta * waveCount * Mathf.PI) *
                spring.Value *
                affectCurve.Evaluate(delta);

            lineRenderer.SetPosition(i,
                Vector3.Lerp(robotFollow.podObject.position,
                currentGrapplePos, delta) + offset);

        }
    }
}

public class Spring
{
    private float strength;
    private float damper;
    private float target;
    private float velocity;
    private float value;

    public void Update(float deltaTime)
    {
        var direction = target - value >= 0 ? 1f : -1f;
        var force = Mathf.Abs(target - value) * strength;
        velocity += (force * direction - velocity * damper) * deltaTime;
        value += velocity * deltaTime;
    }

    public void Reset()
    {
        velocity = 0f;
        value = 0f;
    }

    public void SetValue(float value)
    {
        this.value = value;
    }

    public void SetTarget(float target)
    {
        this.target = target;
    }

    public void SetDamper(float damper)
    {
        this.damper = damper;
    }

    public void SetStrength(float strength)
    {
        this.strength = strength;
    }

    public void SetVelocity(float velocity)
    {
        this.velocity = velocity;
    }

    public float Value => value;
}
