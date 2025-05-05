using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Serialization;

/// <summary>
/// 게임 내 시간 게이지를 관리하는 클래스
/// </summary>
public class GaugeManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GaugeManager Instance { get; private set; }

    public float timeLimit = 30f;         // 타이머 제한 시간
    private float timeRemaining;          // 남은 시간

    [SerializeField] private RectTransform maskTransform;       // 마스크 UI 요소
    [SerializeField] private RectTransform backgroundTransform; // 배경 UI 요소
    
    private float maxWidth;               // 게이지 최대 너비
    private float maxHeight;              // 게이지 최대 높이

    private bool isInitializing = false;  // 초기화 중 여부

    private Coroutine fillCoroutine = null; // 게이지 채우기 코루틴

    private float targetGauge;            // 목표 게이지 값
    public float fillDuration = 0.25f;    // 게이지 채우기 지속 시간

    /// <summary>
    /// 초기화 시 싱글톤 인스턴스 설정
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// 시작 시 게이지 초기화
    /// </summary>
    private void Start()
    {
        InitializeGauge();
    }

    /// <summary>
    /// 타이머 업데이트 함수
    /// </summary>
    /// <param name="currentTime">현재 시간</param>
    public void UpdateTimer(float currentTime)
    {
        AddGauge(currentTime);
    }

    /// <summary>
    /// 게이지 초기화 함수
    /// </summary>
    private void InitializeGauge()
    {
        timeLimit = GameManager.Instance.timeLimit;
        
        if (isInitializing) return;
        isInitializing = true;

        // UI 컴포넌트 참조 가져오기
        if (maskTransform == null)
            maskTransform = transform.Find("Mask")?.GetComponent<RectTransform>();
        if (backgroundTransform == null)
            backgroundTransform = transform.Find("Background")?.GetComponent<RectTransform>();

        if (backgroundTransform != null)
        {
            maxWidth = backgroundTransform.sizeDelta.x;
            maxHeight = backgroundTransform.sizeDelta.y;
        }

        timeRemaining = timeLimit;
        UpdateGaugeWithoutCheck(timeRemaining, timeLimit);

        isInitializing = false;
    }

    /// <summary>
    /// 매 프레임마다 게이지 업데이트
    /// </summary>
    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateGauge(timeRemaining, timeLimit);
        }
        else
        {
            timeRemaining = 0;
            UpdateGauge(timeRemaining, timeLimit);
        }
    }

    /// <summary>
    /// 게이지 업데이트 함수 (체크 포함)
    /// </summary>
    /// <param name="currentTime">현재 시간</param>
    /// <param name="maxTime">최대 시간</param>
    private void UpdateGauge(float currentTime, float maxTime)
    {
        if (maskTransform == null || backgroundTransform == null)
        {
            Debug.LogWarning("GaugeManager에 필요한 컴포넌트가 누락되었습니다. 재초기화를 시도합니다.");
            InitializeGauge();
            return;
        }

        UpdateGaugeWithoutCheck(currentTime, maxTime);
    }

    /// <summary>
    /// 게이지 업데이트 함수 (체크 없음)
    /// </summary>
    /// <param name="currentTime">현재 시간</param>
    /// <param name="maxTime">최대 시간</param>
    private void UpdateGaugeWithoutCheck(float currentTime, float maxTime)
    {
        float factor = Mathf.Clamp01(currentTime / maxTime);
        maskTransform.sizeDelta = new Vector2(maxWidth, factor * maxHeight);
    }

    /// <summary>
    /// 타이머 초기화 함수
    /// </summary>
    public void ResetTimer()
    {
        timeRemaining = timeLimit;
        InitializeGauge();
    }

    /// <summary>
    /// 타이머 지속 시간 설정 함수
    /// </summary>
    /// <param name="newDuration">새 지속 시간</param>
    public void SetTimerDuration(float newDuration)
    {
        timeLimit = newDuration;
        ResetTimer();
    }
    
    /// <summary>
    /// 게이지 추가 함수
    /// </summary>
    /// <param name="amount">추가할 양</param>
    public void AddGauge(float amount)
    {
        targetGauge = Mathf.Min(amount, timeLimit);
        // 이미 실행 중인 코루틴이 있다면 중지
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
        }
        
        // 새 코루틴 시작
        fillCoroutine = StartCoroutine(FillTimeSmooth());
    }

    /// <summary>
    /// 시간 게이지를 부드럽게 채우는 코루틴
    /// </summary>
    /// <returns>대기 시간</returns>
    private IEnumerator FillTimeSmooth()
    {
        float elapsedTime = 0f;
        float startGauge = timeRemaining;
        float endGauge = targetGauge;

        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fillDuration;
            timeRemaining = Mathf.Lerp(startGauge, endGauge, t);
            UpdateGauge(timeRemaining, timeLimit);
            yield return null;
        }

        timeRemaining = endGauge;
        UpdateGauge(timeRemaining, timeLimit);
        fillCoroutine = null;
    }
}