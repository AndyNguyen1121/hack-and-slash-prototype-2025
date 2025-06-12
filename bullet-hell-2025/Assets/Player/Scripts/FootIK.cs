using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    [Header("IK Settings")]
    public bool enableFootIK = true;
    public LayerMask validLayerInteractions;
    public LayerMask stairLayers;

    private Vector3 rightFootPosition, leftFootPosition, rightFootIKPos, leftFootIKPos;
    public float lastPelvisYPos, lastLeftFootYPos, lastRightFootYPos;
    private Quaternion rightFootRotation, leftFootRotation;
    private float previousRotationWeight;

    public float pelvisAdjustmentSpeed = 0.5f;
    public float footHeightAdjustmentSpeed = 0.5f;
    public float footRotationSpeed = 0.5f;
    public float maxRotationSlerp;
    public bool enableRotations = true;
    public Vector3 finalIkPlacementOffset;

    public string leftFootAnimRotationName;
    public string rightFootAnimRotationName;

    public Transform hips;

    [Header("Raycast Settings")]
    public float raycastLength;
    public float raycastDistanceAboveFoot;
    public Vector3 rayCastOffsetXZ;

    [Header("Slope Detection")]
    public float slopeAngle;
    public float slopeRaycastLength;
    public bool onSlope = false;
    public Vector3 slopeDetectionOffset;

    [Header("Step Detection")]
    public float minStepHeightDistance;
    public float stepRaycastLength;
    public bool onSteps = false;
    public Vector3 stepDetectionOffset;

    [Header("Movement Detection")]
    public float idleTime;
    public float minimumTimeOnSlope;
    public bool isRunning;

    [Header("Debug")]
    public bool debugVisible = true;
    private Animator animator;
    public CharacterController characterController;
    public Vector3 localSpaceIkPos;
    public float baseCenterY;


    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = PlayerManager.instance.characterController;
        baseCenterY = characterController.center.y;
    }

    private void Update()
    {
        DetectSteps();
        DetectSlopes(ref slopeAngle);
        DetectIdleTime(ref idleTime);
        CheckIfCanFootIK();

        localSpaceIkPos = transform.InverseTransformPoint(rightFootIKPos);
    }

    private void LateUpdate()
    {
        if (!enableFootIK)
        {
            lastPelvisYPos = PlayerManager.instance.rb.centerOfMass.y;
        }

        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);
;

    }

    public float lastIkPosBeforeDisabled;
    private void OnAnimatorIK(int layerIndex)
    {
            
        FindValidIKPosition(rightFootPosition, HumanBodyBones.RightFoot, AvatarIKGoal.RightFoot, ref rightFootIKPos, ref rightFootRotation);
        FindValidIKPosition(leftFootPosition, HumanBodyBones.LeftFoot, AvatarIKGoal.LeftFoot, ref leftFootIKPos, ref leftFootRotation);

        

        /*if (onSlope)
        {
            Vector3 bodyPos = animator.bodyPosition;
            float center = transform.TransformPoint(characterController.center).y;

            bodyPos.y = center;
            animator.bodyPosition = bodyPos;
        }*/



        if ((onSlope || onSteps) && idleTime < minimumTimeOnSlope)
        {
            lastPelvisYPos = animator.bodyPosition.y;
            return;
        }

        AdjustPelvisPosition();

        if (!enableFootIK)
        {
            return;
        }

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        RotateFoot(leftFootAnimRotationName, AvatarIKGoal.LeftFoot, leftFootRotation);
        RotateFoot(rightFootAnimRotationName, AvatarIKGoal.RightFoot, rightFootRotation);

        MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIKPos, ref lastLeftFootYPos);
        MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIKPos, ref lastRightFootYPos);
    }

    

    #region Solver Methods
    private void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, ref float lastFootPositionY)
    {
        Vector3 footIKPos = animator.GetIKPosition(foot);

        if (positionIkHolder != Vector3.zero)
        {
            positionIkHolder = transform.InverseTransformPoint(positionIkHolder);
            footIKPos = transform.InverseTransformPoint(footIKPos);

            float adjustedYPosition = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, footHeightAdjustmentSpeed * Time.deltaTime);
            footIKPos.y += adjustedYPosition;

            lastFootPositionY = adjustedYPosition;
            footIKPos = transform.TransformPoint(footIKPos);
        }


        animator.SetIKPosition(foot, footIKPos);
    }

    private void RotateFoot(string animRotationParam, AvatarIKGoal foot, Quaternion rotation)
    {
        float lerpedWeight = 0f;

        if (animator.GetFloat(animRotationParam) > 0 && enableRotations)
        {
            animator.SetIKRotation(foot, rotation);
            lerpedWeight = Mathf.Lerp(previousRotationWeight, maxRotationSlerp, footRotationSpeed);
        }
        else
        {
            lerpedWeight = Mathf.Lerp(previousRotationWeight, 0f, footRotationSpeed * Time.deltaTime);
        }

        previousRotationWeight = lerpedWeight;
        animator.SetIKRotationWeight(foot, lerpedWeight);
    }

    
    private void FindValidIKPosition(Vector3 footPosition, HumanBodyBones foot, AvatarIKGoal foot1, ref Vector3 feetIkPositions, ref Quaternion feetIkRotation)
    {
        footPosition.y += raycastDistanceAboveFoot;
        RaycastHit hit;

        Quaternion initalRotation = animator.GetIKRotation(foot1);

        /*if (debugVisible)
        {
            Debug.DrawLine(footPosition + rayCastOffsetXZ, footPosition + (Vector3.down * raycastLength), Color.cyan);
        }*/
        if (Physics.Raycast(footPosition + rayCastOffsetXZ, -transform.up, out hit, raycastLength, stairLayers))
        {
            feetIkPositions = hit.point + finalIkPlacementOffset;
            Quaternion slopeAlignment = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            feetIkRotation = slopeAlignment;
        }
        else if (Physics.Raycast(footPosition + rayCastOffsetXZ, -transform.up, out hit, raycastLength, validLayerInteractions))
        {
            feetIkPositions = hit.point + finalIkPlacementOffset;
            Quaternion slopeAlignment = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            feetIkRotation = slopeAlignment;
        }
        else
        {
            feetIkPositions = Vector3.zero;
        }
    }

    private void AdjustPelvisPosition()
    {
        if (leftFootIKPos == Vector3.zero || rightFootIKPos == Vector3.zero || lastPelvisYPos == 0)
        {
            lastPelvisYPos = animator.bodyPosition.y;
            return;
        }

        // If onSlope → let animation control pelvis
        /*if (onSlope && !animator.GetBool("isMoving"))
        {
            lastPelvisYPos = animator.bodyPosition.y;
            return;
        }*/

        


        float leftFootOffset;
        float rightFootOffset;

        leftFootOffset = leftFootIKPos.y - transform.position.y;
        rightFootOffset = rightFootIKPos.y - transform.position.y;

        float desiredOffset = leftFootOffset < rightFootOffset ? leftFootOffset : rightFootOffset;

        Vector3 desiredPos = animator.bodyPosition + (Vector3.up * desiredOffset);
        desiredPos.y = Mathf.Lerp(lastPelvisYPos, desiredPos.y, pelvisAdjustmentSpeed * Time.deltaTime);
        animator.bodyPosition = desiredPos;
        lastPelvisYPos = desiredPos.y;

    }

    private void AdjustFeetTarget(ref Vector3 footPosition, HumanBodyBones foot)
    {
        footPosition = animator.GetBoneTransform(foot).position;
    }

    private void DetectIdleTime(ref float time)
    {
        if (PlayerInputManager.instance.clampedDirection == Vector2.zero || !PlayerManager.instance.canMove)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }

    }

    private void CheckIfCanFootIK()
    {
        enableFootIK = animator.GetBool("FootIK");
    }

    #endregion

    #region Environment Detection
    private void DetectSlopes(ref float slopeAngle)
    {
        RaycastHit hit;
        if (Physics.Raycast(hips.position + transform.rotation * slopeDetectionOffset, -transform.up, out hit, slopeRaycastLength, validLayerInteractions))
        {
            Vector3 planeVector = Vector3.ProjectOnPlane(-transform.up, hit.normal);
            slopeAngle = Vector3.SignedAngle(planeVector, transform.up, transform.up);
        }
        else
        {
            slopeAngle = 0f;
        }

        onSlope = slopeAngle > 100 && slopeAngle != 0 ? true : false;
    }

    private void DetectSteps()
    {
        RaycastHit hit;

        if (Physics.Raycast(hips.position + transform.rotation * stepDetectionOffset, -transform.up, out hit, stepRaycastLength, validLayerInteractions))
        {
            float stepHeight = transform.InverseTransformPoint(hit.point).y;
            onSteps = Mathf.Abs(transform.position.y - hit.point.y) > minStepHeightDistance;
        }
        else
        {
            onSteps = false;
        }
    }

    public void ActivateFootIKAnimationEvent()
    {
        animator.SetBool("FootIK", true);
    }

    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(rightFootPosition, 0.2f);
        Gizmos.DrawWireSphere(leftFootPosition, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector3(leftFootPosition.x, leftFootPosition.y + raycastDistanceAboveFoot, leftFootPosition.z) + rayCastOffsetXZ, -transform.up * raycastLength);
    }
}
