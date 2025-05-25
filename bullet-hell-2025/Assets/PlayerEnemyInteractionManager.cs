using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System;
public class PlayerEnemyInteractionManager : MonoBehaviour
{
    public PlayerManager playerManager;
    private EnemyInteractionManager currentTarget;

    [Header("Target Detection")]
    public Vector3 detectionOffset;
    public Vector3 detectionSize;

    [Header("Collisions")]
    public Collider[] currentCollisions;
    void Start()
    {
        playerManager = PlayerManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        HandleKnockupFinisher();
    }


    #region KnockupFinisher
    private void HandleKnockupFinisher()
    {
        if (PlayerInputManager.instance.isInteracting && !playerManager.isPerformingAction && playerManager.isGrounded)
        {
            Vector3 boxCenter = transform.position + transform.rotation * detectionOffset;
            Vector3 boxHalfExtents = detectionSize; // Replace with your desired size
            Quaternion boxOrientation = transform.rotation;
            LayerMask layerMask = playerManager.playerCombatManager.whatIsDamageable; // Replace with your desired layers

            currentCollisions = Physics.OverlapBox(boxCenter, boxHalfExtents, boxOrientation, layerMask);

            IgnoreCurrentInteractionCollisions();

            if (currentTarget != null)
            {
                FindAndAlignDistance(2);
                playerManager.playerAnimationManager.rootMotionSpeedMultiplierY = 4;
                playerManager.playerAnimationManager.PlayActionAnimation("KnockupFinisher", true, true, false, false, false, false);
            }
        }
    }

    public void ParentTarget()
    {
        currentTarget.gameObject.transform.parent.SetParent(this.transform, true);
    }

    public void UnparentTarget()
    {
        currentTarget.gameObject.transform.parent.SetParent(null);
    }

    public void KnockupTarget(float height)
    {
        if (currentTarget != null)
        {
            currentTarget.JumpToHeightInTime(0.7f);
        }
    }

    public void SlamTarget(float slamForce)
    {
        if (currentTarget != null)
        {
            currentTarget.SlamDown(slamForce);
        }
    }

    public void FindAndAlignDistance(float distance)
    {
        if (currentTarget == null)
            return;

        Vector3 dir = (currentTarget.transform.position - transform.position).normalized;
        Vector3 desiredPosition = currentTarget.transform.position - (dir * distance);

        dir.y = 0;
        AlignPosition(desiredPosition, dir);
    }

    public void AlignPosition(Vector3 position, Vector3 rotationDir)
    {
        playerManager.characterController.enabled = false;
        Quaternion lookRotation =  Quaternion.LookRotation(rotationDir);

        DOTween.Sequence()
        .Append(transform.DOMove(position, 0.1f).SetEase(Ease.OutSine))
        .Join(transform.DORotateQuaternion(lookRotation, 0.1f).OnComplete(() => playerManager.characterController.enabled = true));
    }
    

    #endregion

    // Animation event
    public void ClearTarget()
    {
        ActivateCurrentInteractionCollisions();
        currentCollisions = null;
        currentTarget = null;
    }

    public void IgnoreCurrentInteractionCollisions()
    {
        foreach (Collider hit in currentCollisions)
        {
            if (hit.CompareTag("Enemy"))
            {
                if (hit.gameObject.TryGetComponent<EnemyInteractionManager>(out currentTarget))
                {
                    playerManager.IgnorePlayerCollider(hit);
                }
            }
        }
    }

    public void ActivateCurrentInteractionCollisions()
    {
        foreach (Collider hit in currentCollisions)
        {
            if (hit.CompareTag("Enemy"))
            {
                if (hit.gameObject.TryGetComponent<EnemyInteractionManager>(out currentTarget))
                {
                    playerManager.EnableCollisionWithPlayerColliders(hit);
                }
            }
        }
    }
    

    private void OnDrawGizmosSelected()
    {
        // Match these with your actual values

        Vector3 boxHalfExtents = detectionSize;   // Replace with your desired half-extents

        // Compute world space center and rotation
        Vector3 boxCenter = transform.position + transform.rotation * detectionOffset;
        Quaternion boxOrientation = transform.rotation;

        // Set Gizmo color
        Gizmos.color = Color.red;

        // Draw the box
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxCenter, boxOrientation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2); // Centered at origin of matrix
    }
}
