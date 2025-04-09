using UnityEngine;

public class GolfBall : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hole"))
        {
            Debug.Log("Ball is in the hole! Game Over!");
            GameOver();
        }
    }

    void GameOver()
    {
        // Display a message or UI
        Debug.Log("Congratulations! You scored!");
        
        // Restart the game after a delay
        Invoke("RestartGame", 2f);
    }

    void RestartGame()
    {
        // You can reload the scene or reset the ball position
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}