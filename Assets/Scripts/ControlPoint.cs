using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    private float xRot = -90f;
    private float yRot = 0f;

    public Rigidbody ball;
    public LineRenderer line;

    public float rotationSpeed = 5f;
    public float shootPower = 30f;
    private const float MinShootPower = 5f;
    private const float MaxShootPower = 60f;
    private const float MinYRotation = -35f;
    private const float MaxYRotation = 83f;


    private float startMouseY;

    void Start()
    {
        transform.position = ball.position;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        transform.position = ball.position;

        if (Input.GetMouseButtonDown(0))
        {
            startMouseY = Input.mousePosition.y;
        }

        if (Input.GetMouseButton(0))
        {
            UpdateRotation();
            float finalPower = CalculateShotPower();
            UpdateLineRenderer(finalPower);
        }

        if (Input.GetMouseButtonUp(0))
        {
            ShootBall();
            ResetAim();
        }
    }

    private void UpdateRotation()
    {
        xRot += Input.GetAxis("Mouse X") * rotationSpeed;
        yRot += Input.GetAxis("Mouse Y") * rotationSpeed;
        yRot = Mathf.Clamp(yRot, MinYRotation, MaxYRotation);
    }

    private float CalculateShotPower()
    {
        float mouseYOffset = startMouseY - Input.mousePosition.y;
        float powerFactor = Mathf.Clamp(mouseYOffset / 5f, 0.5f, 2f);
        return Mathf.Clamp(shootPower * powerFactor, MinShootPower, MaxShootPower);
    }

    private void UpdateLineRenderer(float finalPower)
    {
        Vector3 direction = Quaternion.Euler(yRot, -xRot, 0f) * Vector3.forward;

        line.gameObject.SetActive(true);
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + direction * (finalPower / MaxShootPower) * 8f);
    }

    private void ShootBall()
    {
        Vector3 shotDirection = Quaternion.Euler(yRot, -xRot, 0f) * Vector3.forward;
        float finalPower = CalculateShotPower();
        
        ball.linearVelocity = shotDirection * finalPower;
        line.gameObject.SetActive(false);
    }

    private void ResetAim()
    {
        xRot = -90f;
        yRot = 0f;
    }
}
