using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Serialization;

public class GaugeManager : MonoBehaviour
{
    public static GaugeManager Instance { get; private set; }

    public float timeLimit = 30f;
    private float timeRemaining;

    [SerializeField] private RectTransform maskTransform;
    [SerializeField] private RectTransform backgroundTransform;
    
    private float maxWidth;
    private float maxHeight;

    private bool isInitializing = false;

    private Coroutine fillCoroutine = null;

    private float targetGauge;
    public float fillDuration = 0.25f; 
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

    private void Start()
    {
        InitializeGauge();
    }

    public void UpdateTimer(float currentTime)
    {
        AddGauge(currentTime);
    }
    private void InitializeGauge()
    {
        timeLimit = GameManager.Instance.timeLimit;
        
        if (isInitializing) return;
        isInitializing = true;

      
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

    private void UpdateGauge(float currentTime, float maxTime)
    {
        if (maskTransform == null || backgroundTransform == null)
        {
            Debug.LogWarning("Required components are missing in GaugeManager. Attempting to reinitialize.");
            InitializeGauge();
            return;
        }

        UpdateGaugeWithoutCheck(currentTime, maxTime);
    }

    private void UpdateGaugeWithoutCheck(float currentTime, float maxTime)
    {
        float factor = Mathf.Clamp01(currentTime / maxTime);
        maskTransform.sizeDelta = new Vector2(maxWidth, factor * maxHeight);
    }

    public void ResetTimer()
    {
        timeRemaining = timeLimit;
        InitializeGauge();
    }

    public void SetTimerDuration(float newDuration)
    {
        timeLimit = newDuration;
        ResetTimer();
    }
    
    
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
            UpdateGauge(timeRemaining,timeLimit);
            yield return null;
        }

        timeRemaining = endGauge;
        UpdateGauge(timeRemaining,timeLimit);
        fillCoroutine = null;
    }
}