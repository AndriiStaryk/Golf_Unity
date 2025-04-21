using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ControlPoint : MonoBehaviour
{
    public Rigidbody ball;
    public LineRenderer aimLine;
    public Transform cameraHolder;
    public Vector3 cameraOffset = new Vector3(0, 2.5f, -9);
    public float rotationSpeed = 5f;
    public float gamepadRotationSpeed = 320f;
    public float shootPower = 30f;

    public float isBallMovingThreshold = 1f;
    
    // Gamepad settings
    public float gamepadSensitivity = 40f;
    public float gamepadPowerIncreaseSpeed = 15f;
    
    private float xRot = 0f;
    private float yRot = 0f;
    private float aimXRot = 0f;
    private float aimYRot = 0f;
    private const float MinPower = 5f;
    private const float MaxPower = 75f;
    private const float MinPitch = -15f;
    private const float MaxPitch = 83f;
    private const float MaxAimLineLength = 14f;
    private float currentPower;
    private Transform aimPoint;
    private bool isAiming = false;

    // Input System variables
    private GameControls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool aimPressed;
    private bool shootReleased;
    private bool cancelPressed;

    void Awake()
    {
        // Initialize Input Actions
        controls = new GameControls();
        
        // Set up callbacks for input actions
        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;
        
        controls.Gameplay.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Look.canceled += ctx => lookInput = Vector2.zero;
        
        controls.Gameplay.Aim.performed += ctx => aimPressed = true;
        controls.Gameplay.Aim.canceled += ctx => { aimPressed = false; shootReleased = true; };
        
        controls.Gameplay.Cancel.performed += ctx => cancelPressed = true;
        controls.Gameplay.Cancel.canceled += ctx => cancelPressed = false;
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentPower = MinPower;

        aimPoint = new GameObject("AimPoint").transform;
        aimPoint.SetParent(transform);
        aimPoint.localPosition = Vector3.zero;
        aimPoint.localRotation = Quaternion.identity;
    }

    void Update()
    {
        transform.position = ball.position;

        UpdateInputs();
        
        // Only show aim line when aiming
        aimLine.gameObject.SetActive(isAiming);

        bool isBallMoving = ball.linearVelocity.magnitude > isBallMovingThreshold;
        if (isBallMoving)
        {
            isAiming = false;
            aimLine.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (isAiming)
        {
            UpdateLineRenderer();
        }
    }

    private void UpdateInputs()
    {
        // Handle movement and camera rotation using the new input system
        float mouseX = lookInput.x;
        float mouseY = lookInput.y;
        
        // Apply input sensitivity
        float adjustedMouseX = mouseX * rotationSpeed * Time.deltaTime * 10f; // Multiplied by 10 to match old input system
        float adjustedMouseY = mouseY * rotationSpeed * Time.deltaTime * 10f;
        
        // WASD / Gamepad movement from moveInput
        float moveX = moveInput.x;
        float moveY = moveInput.y;
        
        // Combine inputs for rotation
        xRot += adjustedMouseX;
        yRot += adjustedMouseY * 0.5f;

        aimXRot = xRot;
        aimYRot += adjustedMouseY;

        // Apply movement to rotation
        xRot += moveX * gamepadSensitivity * Time.deltaTime;
        yRot -= moveY * gamepadSensitivity * Time.deltaTime * 0.5f;

        aimXRot += moveX * gamepadSensitivity * Time.deltaTime;
        aimYRot -= moveY * gamepadSensitivity * Time.deltaTime;

        // Clamp angles
        yRot = Mathf.Clamp(yRot, MinPitch * 0.5f, MaxPitch * 0.5f);
        aimYRot = Mathf.Clamp(aimYRot, MinPitch, MaxPitch);

        // Update camera position and rotation
        if (cameraHolder != null)
        {
            cameraHolder.position = Vector3.Lerp(cameraHolder.position, transform.position + transform.rotation * cameraOffset, Time.deltaTime * 10f);
            cameraHolder.rotation = Quaternion.Lerp(cameraHolder.rotation, transform.rotation, Time.deltaTime * 10f);
        }

        transform.rotation = Quaternion.Euler(yRot, xRot, 0f);
        aimPoint.rotation = Quaternion.Euler(aimYRot, aimXRot, 0f);
        
        
        bool canShoot = ball.linearVelocity.magnitude <= isBallMovingThreshold;
        
        // Shooting controls
        if (canShoot)
        {
            if (aimPressed)
            {
                isAiming = true;
                IncreasePower();
            }

            if (shootReleased && isAiming)
            {
                ShootBall();
                ResetAim();
                shootReleased = false;
            }

            // Cancel aiming
            if (cancelPressed)
            {
                ResetAim();
                cancelPressed = false;
            }
        }
    }

    private void IncreasePower()
    {
        float powerIncreaseRate = gamepadPowerIncreaseSpeed;
        currentPower += Time.deltaTime * powerIncreaseRate;
        currentPower = Mathf.Clamp(currentPower, MinPower, MaxPower);
    }

    private void UpdateLineRenderer()
    {
        Vector3 direction = aimPoint.forward;
        
        // Use the ball's position directly
        Vector3 startPos = ball.position;
        Vector3 endPos = startPos + direction * (currentPower / MaxPower) * MaxAimLineLength;
        
        aimLine.SetPosition(0, startPos);
        aimLine.SetPosition(1, endPos);
    }

    private void ShootBall()
    {
        Vector3 shotDirection = aimPoint.forward;
        ball.linearVelocity = shotDirection * currentPower;
        
        // If GameHUD exists, add a shot
        if (GameHUD.Instance != null)
        {
            GameHUD.Instance.AddShot();
        }
        
        isAiming = false;
        aimLine.gameObject.SetActive(false);
        currentPower = MinPower;
    }

    private void ResetAim()
    {
        aimYRot = yRot;
        currentPower = MinPower;
        isAiming = false;
        aimLine.gameObject.SetActive(false);
    }
}