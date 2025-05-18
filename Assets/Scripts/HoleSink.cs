using UnityEngine;
using System.Collections;

public class HoleSink : MonoBehaviour
{
    public Transform holeCenter;
    public float sinkSpeed = 3f;

    void Start()
    {
        holeCenter = transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SinkBall(other.gameObject));
        }
    }

    IEnumerator SinkBall(GameObject ball)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        while (Vector3.Distance(ball.transform.position, holeCenter.position) > 0.05f)
        {
            ball.transform.position = Vector3.MoveTowards(ball.transform.position, holeCenter.position, Time.deltaTime * sinkSpeed);
            yield return null;
        }

        // Add score, effects, sound, etc.
        Debug.Log("🏁 Ball sank in the hole!");
        GameOver();
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameHUD.Instance.ResetGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
