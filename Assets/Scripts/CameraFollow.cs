using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float positionSpeed = 15f;
    public float rotationSpeed = 15f;

    private void LateUpdate()
    {
        if (target == null)
            return;
            
        transform.position = Vector3.Lerp(transform.position, target.position, positionSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotationSpeed * Time.deltaTime);
    }
}