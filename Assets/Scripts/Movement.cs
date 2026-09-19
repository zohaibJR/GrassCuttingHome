using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 2f;

    [Header("Touch")]
    [Tooltip("Drag distance for full speed, as a fraction of the shorter screen side.")]
    [SerializeField, Range(0.05f, 0.4f)]
    private float touchDragRadius = 0.18f;

    [SerializeField, Range(0f, 0.3f)]
    private float touchDeadZone = 0.08f;

    [Header("Smoothing")]
    [SerializeField, Min(1f)]
    private float inputResponsiveness = 18f;

    [SerializeField, Min(1f)]
    private float releaseResponsiveness = 24f;

    private Rigidbody rb;
    private RotateObject rotateObject;

    private InputAction moveAction;
    private InputAction pointerPositionAction;
    private InputAction pointerPressAction;

    private Vector2 pointerStart;
    private Vector3 desiredDirection;
    private Vector3 smoothedDirection;

    public Vector3 CurrentMoveDirection => smoothedDirection;

    private float fixedYPosition;
    private bool canMove = true;
    private bool pointerWasPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rotateObject = GetComponent<RotateObject>();
        fixedYPosition = rb.position.y;

        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        CreateInputActions();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        pointerPositionAction.Enable();
        pointerPressAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        pointerPositionAction.Disable();
        pointerPressAction.Disable();

        pointerWasPressed = false;
        desiredDirection = Vector3.zero;
        smoothedDirection = Vector3.zero;
    }

    private void OnDestroy()
    {
        moveAction.Dispose();
        pointerPositionAction.Dispose();
        pointerPressAction.Dispose();
    }

    private void Update()
    {
        if (!canMove)
            return;

        ReadInput();
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            StopMovement();
            return;
        }

        float responsiveness =
            desiredDirection.sqrMagnitude < 0.0001f
                ? releaseResponsiveness
                : inputResponsiveness;

        float blend =
            1f - Mathf.Exp(
                -responsiveness * Time.fixedDeltaTime
            );

        smoothedDirection = Vector3.Lerp(
            smoothedDirection,
            desiredDirection,
            blend
        );

        if (smoothedDirection.sqrMagnitude < 0.00001f)
            smoothedDirection = Vector3.zero;

        Vector3 targetPosition =
            rb.position +
            smoothedDirection *
            moveSpeed *
            Time.fixedDeltaTime;

        targetPosition.y = fixedYPosition;
        rb.MovePosition(targetPosition);

        if (rotateObject != null)
        {
            rotateObject.SetMoving(
                smoothedDirection.sqrMagnitude > 0.001f
            );
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!value)
        {
            pointerWasPressed = false;
            desiredDirection = Vector3.zero;
            StopMovement();
        }
    }

    private void ReadInput()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        bool pointerIsPressed = pointerPressAction.IsPressed();

        if (pointerIsPressed)
        {
            Vector2 pointerPosition =
                pointerPositionAction.ReadValue<Vector2>();

            if (!pointerWasPressed)
            {
                pointerStart = pointerPosition;
                input = Vector2.zero;
            }
            else
            {
                float screenSide = Mathf.Min(
                    Screen.width,
                    Screen.height
                );

                float radius = Mathf.Max(
                    1f,
                    screenSide * touchDragRadius
                );

                Vector2 drag =
                    (pointerPosition - pointerStart) / radius;

                float magnitude = drag.magnitude;

                if (magnitude <= touchDeadZone)
                {
                    input = Vector2.zero;
                }
                else
                {
                    float strength =
                        (Mathf.Min(magnitude, 1f) -
                         touchDeadZone) /
                        (1f - touchDeadZone);

                    input =
                        drag / magnitude * strength;
                }
            }
        }

        pointerWasPressed = pointerIsPressed;

        input = Vector2.ClampMagnitude(input, 1f);
        desiredDirection = new Vector3(
            input.x,
            0f,
            input.y
        );
    }

    private void StopMovement()
    {
        desiredDirection = Vector3.zero;
        smoothedDirection = Vector3.zero;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0f;
        velocity.z = 0f;
        rb.linearVelocity = velocity;

        if (rotateObject != null)
            rotateObject.SetMoving(false);
    }

    private void CreateInputActions()
    {
        moveAction = new InputAction(
            "Move",
            InputActionType.Value
        );

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        moveAction.AddBinding("<Gamepad>/leftStick");

        pointerPositionAction = new InputAction(
            "Pointer Position",
            InputActionType.Value,
            "<Pointer>/position"
        );

        pointerPressAction = new InputAction(
            "Pointer Press",
            InputActionType.Button,
            "<Pointer>/press"
        );
    }

    public void SetGroundHeight(float height)
    {
        fixedYPosition = height;

        Vector3 position = rb.position;
        position.y = height;
        rb.position = position;
    }
}