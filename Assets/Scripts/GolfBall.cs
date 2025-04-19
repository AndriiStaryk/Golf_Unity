using UnityEngine;

public class GolfBall : MonoBehaviour
{
    public GameHUD gameHUD;
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
        gameHUD.ResetGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}