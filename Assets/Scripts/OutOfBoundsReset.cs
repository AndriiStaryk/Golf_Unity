using UnityEngine;
public class OutOfBoundsReset : MonoBehaviour
{
    public float minYPosition = -5f;
    public string ballTag = "Player"; // Make sure your ball has this tag
    
    private Vector3 startPosition;
    private GameObject ball;
    
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag(ballTag);
        if (ball != null)
        {
            startPosition = ball.transform.position;
        }
    }
    
    void Update()
    {
        if (ball != null)
        {
            // If ball falls below minimum Y position
            if (ball.transform.position.y < minYPosition)
            {
                ResetBall();
            }
        }
    }
    
    void ResetBall()
    {
        // Reset ball position and velocity
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            ball.transform.position = startPosition;
        }
        
        // Add penalty to score or show message
        Debug.Log("Ball out of bounds! Returning to start position.");
    }
}