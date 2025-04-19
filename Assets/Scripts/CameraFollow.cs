using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float speed = 15f;

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, speed * Time.deltaTime);
    }
}
