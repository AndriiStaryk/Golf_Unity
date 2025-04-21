using TMPro;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    public Rigidbody ball;
    public LineRenderer aimLine;
    public Transform cameraHolder;
    public Vector3 cameraOffset = new Vector3(0, 2.5f, -9);
    public float rotationSpeed = 5f;
    public float shootPower = 30f;
    public float ballStoppedThreshold = 0.1f; 
    
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
    private bool isBallMoving = false;

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
        
        isBallMoving = ball.linearVelocity.magnitude > ballStoppedThreshold;
        
        UpdateRotation();

        if (!isBallMoving)
        {
            if (Input.GetMouseButton(0))
            {
                IncreasePower();
                aimLine.gameObject.SetActive(true);
            }

            if (Input.GetMouseButtonUp(0))
            {
                ShootBall();
                ResetAim();
            }

            if (Input.GetMouseButtonUp(1))
            {
                ResetAim();
            }
        }
        else
        {
            // Hide aim line when ball is moving
            aimLine.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0) && !isBallMoving)
        {
            UpdateLineRenderer();
        }
        
        // Update camera position smoothly
        if (cameraHolder != null)
        {
            cameraHolder.position = Vector3.Lerp(cameraHolder.position, 
                transform.position + transform.rotation * cameraOffset, 
                Time.deltaTime * 10f);
            cameraHolder.rotation = Quaternion.Lerp(cameraHolder.rotation, 
                transform.rotation, 
                Time.deltaTime * 10f);
        }
    }

    private void UpdateRotation()
    {
        xRot += Input.GetAxis("Mouse X") * rotationSpeed;
        yRot += Input.GetAxis("Mouse Y") * rotationSpeed * 0.5f;

        aimXRot = xRot;
        aimYRot += Input.GetAxis("Mouse Y") * rotationSpeed;

        xRot += ((Input.GetKey(KeyCode.A) ? -0.5f : 0f) + (Input.GetKey(KeyCode.D) ? 0.5f : 0f));
        yRot -= ((Input.GetKey(KeyCode.W) ? 0.25f : 0f) + (Input.GetKey(KeyCode.S) ? -0.25f : 0f));

        aimXRot += ((Input.GetKey(KeyCode.A) ? -0.5f : 0f) + (Input.GetKey(KeyCode.D) ? 0.5f : 0f));
        aimYRot -= ((Input.GetKey(KeyCode.W) ? 0.5f : 0f) + (Input.GetKey(KeyCode.S) ? -0.5f : 0f));

        yRot = Mathf.Clamp(yRot, MinPitch * 0.5f, MaxPitch * 0.5f);
        aimYRot = Mathf.Clamp(aimYRot, MinPitch, MaxPitch);

        transform.rotation = Quaternion.Euler(yRot, xRot, 0f);
        aimPoint.rotation = Quaternion.Euler(aimYRot, aimXRot, 0f);
    }

    private void IncreasePower()
    {
        currentPower += Time.deltaTime * 20;
        currentPower = Mathf.Clamp(currentPower, MinPower, MaxPower);
    }

    private void UpdateLineRenderer()
    {
        Vector3 direction = aimPoint.forward;
        
        // Use the ball's position directly instead of the transform's position
        Vector3 startPos = ball.position;
        Vector3 endPos = startPos + direction * (currentPower / MaxPower) * MaxAimLineLength;
        
        aimLine.SetPosition(0, startPos);
        aimLine.SetPosition(1, endPos);
    }

    private void ShootBall()
    {
        Vector3 shotDirection = aimPoint.forward;
        ball.linearVelocity = shotDirection * currentPower;
        GameHUD.Instance.AddShot();
        aimLine.gameObject.SetActive(false);
        currentPower = MinPower;
    }

    private void ResetAim()
    {
        aimYRot = yRot;
        currentPower = MinPower;
        aimLine.gameObject.SetActive(false);
    }
}