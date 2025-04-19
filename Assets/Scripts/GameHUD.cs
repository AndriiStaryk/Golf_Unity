using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
public static GameHUD Instance { get; private set; }

void Awake()
{
    Instance = this;
}

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI shotsText;

    private float timer = 0f;
    private int shots = 0;
    private bool gameStarted = true;

    void Update()
    {
        if (gameStarted)
        {
            timer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }

        shotsText.text = $"Shots: {shots}";
    }

    public void AddShot()
    {
        shots++;
    }

    public void ResetGame()
    {
        timer = 0f;
        shots = 0;
        gameStarted = true;
    }

    public void StopTimer()
    {
        gameStarted = false;
    }
}
