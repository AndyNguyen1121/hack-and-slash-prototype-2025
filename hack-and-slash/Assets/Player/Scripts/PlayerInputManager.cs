using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

#region InputID
public enum InputID
{
    Attack,
    Interact,
    Jump,
    Sprint,
    Dash,
    LockOn,
    Guard
}

#endregion
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    public PlayerControls inputActions;
    public PlayerManager playerManager;
    public float deadZone = 0.2f;

    [Header("Movement")]
    public Vector2 movementDirection;
    public Vector2 clampedDirection;
    public float horizontalInput;
    public float verticalInput;
    public float moveAmount;
    public float joystickIdleTime;

    [Header("Press flags")]
    public bool sprintPressed = false;
    public bool isSprinting = false;
    public bool isInteracting = false;
    public bool jumpPressed = false;
    public bool attackPressed = false;
    public bool dodgePressed = false;
    public bool isGuarding = false;
    public bool pauseButtonPressed = false;

    [Header("Input Queue")]
    public float defaultQueueTimer = 0.35f;
    public float queueTimer = 0f;
    public bool queueIsActive = false;
    public bool attackInputQueue = false;

    // Assign in Inspector
    [Header("Camera Controls")]
    public CinemachineInputAxisController cameraInputProvider;

    [Header("Input ID Information")]
    public List<InputID> currentInputs = new();

    [Header("Input Direction Relative To Player")]
    public Vector2 localInputDirection;

    private float timeSinceLoadedIn = 0;

    private InputSystemUIInputModule eventSystemInputModule;
    public InputDevice currentDevice;

    private void Awake()

    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 300;
    }

    private void Start()
    {
        playerManager = PlayerManager.instance;
        eventSystemInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
        

        HandleMovementInput();
        HandleSprintingInput();
        HandleInteractInput();
        HandleJumpingInput();
        HandleAttackInput();
        HandlePauseInput();
        HandleDodgeInput();
        HandleQueuedInputs();
        HandleJoystickIdleTime();
        CalculateInputDirectionRelativeToPlayer();

        HandleTimeLoadedIn();
    }

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerControls();

            inputActions.MovementMap.Movement.performed += ctx => movementDirection = ctx.ReadValue<Vector2>();
            inputActions.Actions.AttackQueue.performed += ctx => QueueInput(ref attackInputQueue);
            inputActions.Camera.LockOnToggle.started += ctx => HandleLockOnInput(true);
            inputActions.Camera.LockOnToggle.canceled += ctx => HandleLockOnInput(false);
            inputActions.Camera.LockOnSwitch.performed += ctx => playerManager.playerCameraManager.SwitchLockOnTargets(ctx.ReadValue<Vector2>().x);
            inputActions.Actions.Guard.started += ctx =>
            {
                isGuarding = true;
                AddInput(InputID.Guard);
            };

            inputActions.Actions.Guard.canceled += ctx => 
            {
                isGuarding = false;
                RemoveInput(InputID.Guard);
            };

            InputSystem.onActionChange += InputActionChangeCallback;
        }

        inputActions.Enable();
        cameraInputProvider.enabled = true;
    }

    private void InputActionChangeCallback(object obj, InputActionChange change)
    {
        /*if (change != InputActionChange.ActionPerformed) return;

        if (obj is InputAction receivedInputAction)
        {
            InputDevice lastDevice = receivedInputAction.activeControl.device;

            if (lastDevice != currentDevice && lastDevice is Gamepad)
            {
                eventSystemInputModule.deselectOnBackgroundClick = false;
                currentDevice = lastDevice;
                Debug.Log("Gamepad detected - deselect disabled");
            }
            else if ((lastDevice != currentDevice) && lastDevice is Keyboard)
            {
                eventSystemInputModule.deselectOnBackgroundClick = true;
                currentDevice = lastDevice;
                Debug.Log("Keyboard/Mouse detected - deselect enabled");
            }
        }*/
    }

    private void OnDisable()
    {
        movementDirection = Vector2.zero;
        playerManager.playerAnimationManager.UpdateAnimationMovementParameters(0, 0);
        cameraInputProvider.enabled = false;
        inputActions.Disable();
        
    }

    public void HandleLockOnInput(bool enabled)
    {
        if (enabled)
        {
            AddInput(InputID.LockOn);
        }
        else
        {
            RemoveInput(InputID.LockOn);
        }

        if ((!playerManager.playerCameraManager.isLockedOn && enabled)) 
        {
            playerManager.playerCameraManager.ToggleLockOn();
        }
        else if (playerManager.playerCameraManager.isLockedOn && !enabled)
        { 
            
            playerManager.playerCameraManager.ToggleLockOn();
        }

        
    }

    private void HandleInteractInput()
    {
        isInteracting = inputActions.Actions.Interact.WasPressedThisFrame();

        if (isInteracting) 
            AddInput(InputID.Interact);
        else
            RemoveInput(InputID.Interact);
    }

    private void HandleJumpingInput()
    {
        jumpPressed = inputActions.MovementMap.Jump.WasPressedThisFrame();

        if (jumpPressed)
            AddInput(InputID.Jump);
        else
            RemoveInput(InputID.Jump);
    }

    private void HandleSprintingInput()
    {
        if (sprintPressed && moveAmount > 0) 
        {
            isSprinting = true;
            AddInput(InputID.Sprint);
        }
        else
        {
            isSprinting = false;
            RemoveInput(InputID.Sprint);
        }
    }
    private void HandleAttackInput()
    {
        attackPressed = inputActions.Actions.Attack.WasPressedThisFrame();

        if (attackPressed)
            AddInput(InputID.Attack);
        else
            RemoveInput(InputID.Attack);
    }

    private void HandleDodgeInput()
    {
        dodgePressed = inputActions.MovementMap.Dodge.WasPressedThisFrame();

        if (dodgePressed)
            AddInput(InputID.Dash);
        else
            RemoveInput(InputID.Dash);
    }

    private void HandleMovementInput()
    {
        horizontalInput = movementDirection.x;
        verticalInput = movementDirection.y;

        float clampedHorizontal = 0f;
        float clampedVertical = 0f;


        if (Mathf.Abs(movementDirection.x) > 0.5)
        {
            clampedHorizontal = movementDirection.x > 0 ? 1 : -1f;
        }
        else if (Mathf.Abs(movementDirection.x) > deadZone)
        {
            clampedHorizontal = movementDirection.x > 0 ? 0.5f : -0.5f;
        }

        if (Mathf.Abs(movementDirection.y) > 0.5)
        {
            clampedVertical = movementDirection.y > 0 ? 1 : -1f;
        }
        else if (Mathf.Abs(movementDirection.y) > deadZone)
        {
            clampedVertical = movementDirection.y > 0 ? 0.5f : -0.5f;
        }

        moveAmount = Mathf.Clamp01(Mathf.Abs(clampedVertical) + Mathf.Abs(clampedHorizontal));

        if (moveAmount <= 0.5 && moveAmount > deadZone)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = isSprinting ? 1.5f : 1f;
        }
        else
        {
            moveAmount = 0;
        }

        clampedDirection.x = clampedHorizontal;
        clampedDirection.y = clampedVertical;

        if (!playerManager.playerCameraManager.isLockedOn)
        {
            playerManager.playerAnimationManager.UpdateAnimationMovementParameters(0, moveAmount);
        }
        else
        {
            float adjustedMovementX = clampedHorizontal != 0 ? 0.5f : 0;
            float adjustedMovementY = clampedVertical != 0 ? 0.5f : 0;

            if (clampedHorizontal < 0)
            {
                adjustedMovementX *= -1;
            }
            if (clampedVertical < 0)
            {
                adjustedMovementY *= -1; 
            }

            playerManager.playerAnimationManager.UpdateAnimationMovementParameters(adjustedMovementX, adjustedMovementY);
        }
    }

    private void HandleJoystickIdleTime()
    {
        if (clampedDirection == Vector2.zero)
        {
            joystickIdleTime += Time.deltaTime;
        }
        else
        {
            joystickIdleTime = 0;
        }
    }
    
    private void HandlePauseInput()
    {
        pauseButtonPressed = inputActions.UI.Pause.WasPressedThisFrame();
    }
    private void QueueInput(ref bool queuedInput)
    {
        attackInputQueue = false;

        if (playerManager.isPerformingAction)
        {
            queuedInput = true;
            queueTimer = defaultQueueTimer;
            queueIsActive = true;
        }
    }

    private void ProcessQueuedInput()
    {
        if (attackInputQueue)
        {
            attackPressed = true;
            AddInput(InputID.Attack);
        }
    }

    private void HandleQueuedInputs()
    {
        if (queueIsActive)
        {
            if (queueTimer > 0)
            {
                queueTimer -= Time.deltaTime;
                ProcessQueuedInput();
            }
            else
            {
                attackInputQueue = false;
                queueIsActive = false;
            }
        }
    }

    public void AddInput(InputID id)
    {
        if (!currentInputs.Contains(id))
        {
            currentInputs.Add(id);
        }
    }

    public void RemoveInput(InputID id)
    {
        if (currentInputs.Contains(id))
        {
            currentInputs.Remove(id);
        }
    }
    
    public void CalculateInputDirectionRelativeToPlayer()
    {
        if (clampedDirection == Vector2.zero)
        {
            localInputDirection = Vector2.zero;
            return;
        }
        Vector3 playerDirection = playerManager.mainCam.transform.right * movementDirection.x;
        playerDirection += playerManager.mainCam.transform.forward * movementDirection.y;

        playerDirection.y = 0;
        playerDirection.Normalize();

        Vector3 dir = transform.InverseTransformDirection(playerDirection);

        
        localInputDirection = new Vector2(dir.x, dir.z);

        if (Mathf.Abs(localInputDirection.x) > 0.5f)
        {
            localInputDirection.x = localInputDirection.x > 0 ? 1 : -1f;
        }
        else
        {
            localInputDirection.x = 0;
        }

        if (Mathf.Abs(localInputDirection.y) > 0.8f)
        {
            localInputDirection.y = localInputDirection.y > 0 ? 1 : -1f;
        }
        else
        {
            localInputDirection.y = 0;
        }

    }

    public void HandleTimeLoadedIn()
    {
        timeSinceLoadedIn += Time.deltaTime;

        if (timeSinceLoadedIn < 0.25)
        {
            jumpPressed = false;
        }
    }

    public void DisableAllInputsExceptUI()
    {
        inputActions.Camera.Disable();
        inputActions.MovementMap.Disable();
        inputActions.Actions.Disable();
        inputActions.UI.Enable();
    }

    public void ToggleInputs(bool camera, bool movement, bool actions, bool ui)
    {
        if (camera)
            inputActions.Camera.Disable();
        else
            inputActions.Camera.Enable();

        if (movement)
            inputActions.MovementMap.Disable();
        else
            inputActions.MovementMap.Enable();

        if (actions)
            inputActions.Actions.Disable();
        else
            inputActions.Actions.Enable();

        if (ui)
            inputActions.UI.Disable();
        else
            inputActions.UI.Enable();
    }

    public void EnableAllInputs()
    {
        inputActions.Camera.Enable();
        inputActions.MovementMap.Enable();
        inputActions.Actions.Enable();
        inputActions.UI.Enable();
    }
}
