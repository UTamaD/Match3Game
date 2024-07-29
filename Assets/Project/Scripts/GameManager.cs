using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public enum GameState
{
    NotStarted,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float timeLimit = 120f; 
    public TextMeshProUGUI timerText; 
    public GameObject gameOverPanel; 

    private GameState currentState;
    private float remainingTime;
    private bool isGamePaused = false;


    public Button restartBtn;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
        SetState(GameState.NotStarted);
        StartGame();

    }

    public void AddTime(float time)
    {
        remainingTime += time;
        remainingTime = Mathf.Min(remainingTime,timeLimit);
        UpdateTimerDisplay();

        if (GaugeManager.Instance != null)
        {
            GaugeManager.Instance.UpdateTimer(remainingTime);
        }
    }
    private void Update()
    {
        if (currentState == GameState.Playing && !isGamePaused)
        {
            UpdateTimer();
        }
    }

    public void StartTimer(float duration)
    {
        GaugeManager.Instance.SetTimerDuration(duration);
    }
    
    public void ResetTimer()
    {
        GaugeManager.Instance.ResetTimer();
    }
    public void StartGame()
    {
        SetState(GameState.Playing);
        remainingTime = timeLimit;
        StartTimer(remainingTime);
        UpdateTimerDisplay();
        gameOverPanel.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        if (GaugeManager.Instance != null)
        {
            GaugeManager.Instance.ResetTimer();
        }
        
        
        SetState(GameState.Playing);
    }

    public void EndGame()
    {
        SetState(GameState.GameOver);
        gameOverPanel.SetActive(true);
        if (restartBtn != null)
        {
            restartBtn.onClick.AddListener(RestartGame);
        }
        // Optionally show final score or other end-game information
    }

    private void SetState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.NotStarted:
                // Initialize game setup or show start menu
                break;
            case GameState.Playing:
                // Start or resume game logic
                Time.timeScale = 1; // Ensure the game is running
                break;
            case GameState.Paused:
                // Pause game logic
                Time.timeScale = 0; // Freeze game time
                break;
            case GameState.GameOver:
                // Handle game over logic
                Time.timeScale = 0; // Freeze game time
                break;
        }
    }

    private void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            EndGame(); // End the game when time runs out
        }
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(remainingTime).ToString();
        }
    }

    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            SetState(GameState.Paused);
            isGamePaused = true;
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            SetState(GameState.Playing);
            isGamePaused = false;
        }
    }


}
