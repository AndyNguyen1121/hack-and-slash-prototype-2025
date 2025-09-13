using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour
{
    private PlayerManager playerManager;

    [Header("Camera Collection")]
    public CinemachineCamera normalCam;
    public CinemachineCamera lockOnCam;
    CinemachineVirtualCameraBase[] cameras;
    public Camera mainCam;
    public CinemachineDecollider cameraCollider;

    [Header("Lock On Attributes")]
    public bool isLockedOn = false;
    public Transform currentLockOnTarget;
    public EnemyManager lockedOnEnemy;
    public float lockOnRadius;
    public float lockOnAngle = 70;
    public List<Transform> validLockOnTargets = new List<Transform>();
    public Transform cursorPosition;

    [Header("Layer Mask")]
    public LayerMask whatIsEnemy;
    public LayerMask whatIsWallCheckable;

    [Header("Wall Check")]
    public float wallCheckRadius = 2f;
    public bool collidingWithWall;

    private void Awake()
    {
        cameras = new CinemachineVirtualCameraBase[] { normalCam, lockOnCam };
        
    }
    private void Start()
    {
        cameraCollider = lockOnCam.GetComponent<CinemachineDecollider>();
        playerManager = PlayerManager.instance;
        mainCam = Camera.main;
    }

    public void SwitchCameras(int camNum, Transform target = null)
    {
        if (camNum < 0)
            return;



        for (int i = 0; i < cameras.Length; i++)
        {
            if (i == camNum)
            {

                if (cameras[i].name == lockOnCam.name && target != null)
                {
                    currentLockOnTarget = target;
                    isLockedOn = true;
                    playerManager.animator.SetBool("lockedOn", true);

                    LockOnTarget lockOnScript = currentLockOnTarget.GetComponent<LockOnTarget>();
                    lockedOnEnemy = currentLockOnTarget.GetComponent<EnemyManager>();

                    if (lockOnScript != null)
                    {
                        LockOnUI.instance.ChangeTarget(lockOnScript.cursorPosition);
                        cameras[i].LookAt = lockOnScript.cursorPosition;
                        cursorPosition = lockOnScript.cursorPosition;
                    }
                    else
                        Debug.LogWarning("No LockOnTarget component on object.");
                }
                else
                {
                    isLockedOn = false;
                    playerManager.animator.SetBool("lockedOn", false);
                    LockOnUI.instance.ChangeTarget(null);
                    currentLockOnTarget = null;
                    cursorPosition = null;
                    lockedOnEnemy = null;
                }

                cameras[i].Priority = 10;
            }
            else
            {
                cameras[i].Priority = 0;
            }
        }

    }

    private void FindValidLockOnTargets()
    {
        validLockOnTargets.Clear();

        Collider[] targets = Physics.OverlapSphere(transform.position, lockOnRadius, whatIsEnemy);

        foreach (Collider target in targets)
        {
            LockOnTarget lockOnTarget = target.GetComponentInChildren<LockOnTarget>();

            if (lockOnTarget != null && lockOnTarget.enabled)
            {
                Vector3 directionRelativeToCamera = lockOnTarget.gameObject.transform.position - mainCam.transform.position;
                float angleRelativeToCamera = Vector3.Angle(mainCam.transform.forward, directionRelativeToCamera);

                if (angleRelativeToCamera > 0 && angleRelativeToCamera <= lockOnAngle)
                {
                    validLockOnTargets.Add(lockOnTarget.transform);
                }
            }
        }

    }

    public void ToggleLockOn()
    {
        if (!isLockedOn)
        {
            FindValidLockOnTargets();

            float closestDistance = Mathf.Infinity;
            Transform closestTarget = null;

            foreach (Transform target in validLockOnTargets)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, target.position);

                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    closestTarget = target;
                }
            }

            if (closestTarget != null)
            {
                SwitchCameras(1, closestTarget);
            }
        }
        else
        {
            currentLockOnTarget = null;
            SwitchCameras(0);
        }
    }

    public void SwitchLockOnTargets(float xInput)
    {
        if (isLockedOn && currentLockOnTarget != null && xInput != 0)
        {
            FindValidLockOnTargets();

            Transform closestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (Transform target in validLockOnTargets)
            {
                if (target != currentLockOnTarget)
                {
                    Vector3 directionRelativeToCamera = target.position - mainCam.transform.position;
                    float angleRelativeToCamera = Vector3.Angle(mainCam.transform.forward, directionRelativeToCamera);
                    float distance = Vector3.Distance(target.position, currentLockOnTarget.position);

                    if (angleRelativeToCamera > 0 && angleRelativeToCamera <= lockOnAngle && distance < closestDistance)
                    {
                        if ((xInput < 0 && Vector3.Dot(mainCam.transform.right, directionRelativeToCamera) < 0) || // Check Left OR
                            (xInput > 0 && Vector3.Dot(mainCam.transform.right, directionRelativeToCamera) > 0))   // Check Right depending on xInput
                        {
                            closestTarget = target;
                            closestDistance = distance;
                        }
                    }
                }
            }

            if (closestTarget != null)
            {
                SwitchCameras(1, closestTarget);
            }
        }
    }

    public void DisableCurrentTarget()
    {
        SwitchCameras(0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lockOnRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wallCheckRadius);
    }
}