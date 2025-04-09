using UnityEngine;
using UnityEngine.UI;

public class ControlPoint : MonoBehaviour
{
    private float xRot = -90f;
    private float yRot = 0f;

    public Rigidbody ball;
    public LineRenderer line;
    public Slider powerBar;

    public float rotationSpeed = 5f;
    public float shootPower = 30f;
    private const float MinShootPower = 5f;
    private const float MaxShootPower = 60f;
    private const float MinYRotation = -35f;
    private const float MaxYRotation = 83f;

    private float currentPower;

    void Start()
    {
        transform.position = ball.position;
        transform.rotation = Quaternion.identity;
        currentPower = MinShootPower;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        transform.position = ball.position;
        UpdateRotation();
        UpdatePowerBar();

        if (Input.GetMouseButton(0))
        {
            IncreasePower();
            UpdateLineRenderer();
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

    private void UpdateRotation()
    {
        xRot += Input.GetAxis("Mouse X") * rotationSpeed;
        yRot += Input.GetAxis("Mouse Y") * rotationSpeed;

        // Fine-tune aiming with WASD
        xRot += ((Input.GetKey(KeyCode.A) ? 1 : 0) - (Input.GetKey(KeyCode.D) ? 1 : 0)) * 0.5f;
        yRot += ((Input.GetKey(KeyCode.S) ? 1 : 0) - (Input.GetKey(KeyCode.W) ? 1 : 0)) * 0.5f;

        yRot = Mathf.Clamp(yRot, MinYRotation, MaxYRotation);
        transform.localRotation = Quaternion.Euler(yRot, xRot, 0.0f);
    }

    private void IncreasePower()
    {
        currentPower += Time.deltaTime * 20;
        currentPower = Mathf.Clamp(currentPower, MinShootPower, MaxShootPower);
    }

    private void UpdatePowerBar()
    {
        if (powerBar)
        {
            powerBar.value = currentPower / MaxShootPower;
        }
    }

    private void UpdateLineRenderer()
    {
        Vector3 direction = Quaternion.Euler(yRot, -xRot, 0f) * Vector3.forward;
        line.gameObject.SetActive(true);
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + direction * (currentPower / MaxShootPower) * 8f);
    }

    private void ShootBall()
    {
        Vector3 shotDirection = Quaternion.Euler(yRot, -xRot, 0f) * Vector3.forward;
        ball.linearVelocity = shotDirection * currentPower;
        line.gameObject.SetActive(false);
        currentPower = MinShootPower;
    }

    private void ResetAim()
    {
        xRot = -90f;
        yRot = 0f;
        currentPower = MinShootPower;
        line.gameObject.SetActive(false);
    }
}
