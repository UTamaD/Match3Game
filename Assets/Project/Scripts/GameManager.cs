using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

/// <summary>
/// 게임 상태를 나타내는 열거형
/// </summary>
public enum GameState
{
    NotStarted,  // 시작되지 않음
    Playing,     // 게임 진행 중
    Paused,      // 일시 정지됨
    GameOver     // 게임 종료됨
}

/// <summary>
/// 게임 전체 상태와 흐름을 관리하는 클래스
/// </summary>
public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    [Header("게임 설정")]
    public float timeLimit = 120f;           // 게임 제한 시간(초)
    public TextMeshProUGUI timerText;        // 타이머 표시 UI 텍스트
    public GameObject gameOverPanel;         // 게임 오버 패널

    private GameState currentState;          // 현재 게임 상태
    private float remainingTime;             // 남은 시간
    private bool isGamePaused = false;       // 게임 일시정지 여부


    public Button restartBtn;                // 재시작 버튼
    
    /// <summary>
    /// 초기화 시 싱글톤 인스턴스 설정
    /// </summary>
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

    /// <summary>
    /// 게임 시작 시 초기 설정
    /// </summary>
    private void Start()
    {
        SetState(GameState.NotStarted);
        StartGame();
    }

    /// <summary>
    /// 게임 시간을 추가하는 함수
    /// </summary>
    /// <param name="time">추가할 시간(초)</param>
    public void AddTime(float time)
    {
        remainingTime += time;
        remainingTime = Mathf.Min(remainingTime, timeLimit);
        UpdateTimerDisplay();

        if (GaugeManager.Instance != null)
        {
            GaugeManager.Instance.UpdateTimer(remainingTime);
        }
    }

    /// <summary>
    /// 매 프레임마다 타이머 업데이트
    /// </summary>
    private void Update()
    {
        if (currentState == GameState.Playing && !isGamePaused)
        {
            UpdateTimer();
        }
    }

    /// <summary>
    /// 타이머 시작
    /// </summary>
    /// <param name="duration">타이머 지속 시간</param>
    public void StartTimer(float duration)
    {
        GaugeManager.Instance.SetTimerDuration(duration);
    }
    
    /// <summary>
    /// 타이머 초기화
    /// </summary>
    public void ResetTimer()
    {
        GaugeManager.Instance.ResetTimer();
    }

    /// <summary>
    /// 게임 시작 함수
    /// </summary>
    public void StartGame()
    {
        SetState(GameState.Playing);
        remainingTime = timeLimit;
        StartTimer(remainingTime);
        UpdateTimerDisplay();
        gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// 게임 재시작 함수
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        if (GaugeManager.Instance != null)
        {
            GaugeManager.Instance.ResetTimer();
        }
        
        SetState(GameState.Playing);
    }

    /// <summary>
    /// 게임 종료 함수
    /// </summary>
    public void EndGame()
    {
        SetState(GameState.GameOver);
        gameOverPanel.SetActive(true);
        if (restartBtn != null)
        {
            restartBtn.onClick.AddListener(RestartGame);
        }
    }

    /// <summary>
    /// 게임 상태 설정 함수
    /// </summary>
    /// <param name="newState">새로운 게임 상태</param>
    private void SetState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.NotStarted:
                // 게임 시작 전 초기화
                break;
            case GameState.Playing:
                // 게임 실행 또는 재개
                Time.timeScale = 1; // 게임 시간 정상 진행
                break;
            case GameState.Paused:
                // 게임 일시 정지
                Time.timeScale = 0; // 게임 시간 정지
                break;
            case GameState.GameOver:
                // 게임 종료 처리
                Time.timeScale = 0; // 게임 시간 정지
                break;
        }
    }

    /// <summary>
    /// 타이머 업데이트 함수
    /// </summary>
    private void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            EndGame(); // 시간이 다 되면 게임 종료
        }
        UpdateTimerDisplay();
    }

    /// <summary>
    /// 타이머 UI 업데이트 함수
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(remainingTime).ToString();
        }
    }

    /// <summary>
    /// 게임 일시 정지 함수
    /// </summary>
    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            SetState(GameState.Paused);
            isGamePaused = true;
        }
    }

    /// <summary>
    /// 게임 재개 함수
    /// </summary>
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            SetState(GameState.Playing);
            isGamePaused = false;
        }
    }
}
