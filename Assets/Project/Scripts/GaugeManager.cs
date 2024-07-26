using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour
{
    public static GaugeManager Instance { get; private set; }

    public float timerDuration = 30f;
    private float timeRemaining;

    [SerializeField] private RectTransform maskTransform;
    [SerializeField] private RectTransform backgroundTransform;
    
    private float maxWidth;
    private float maxHeight;

    private bool isInitializing = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        InitializeGauge();
    }
    public void UpdateTimer(float currentTime)
    {
        timeRemaining = currentTime;
        UpdateGauge(timeRemaining, timerDuration);
    }
    private void InitializeGauge()
    {
        if (isInitializing) return;
        isInitializing = true;

        // Find references if they are not set
        if (maskTransform == null)
            maskTransform = transform.Find("Mask")?.GetComponent<RectTransform>();
        if (backgroundTransform == null)
            backgroundTransform = transform.Find("Background")?.GetComponent<RectTransform>();

        if (backgroundTransform != null)
        {
            maxWidth = backgroundTransform.sizeDelta.x;
            maxHeight = backgroundTransform.sizeDelta.y;
        }

        timeRemaining = timerDuration;
        UpdateGaugeWithoutCheck(timeRemaining, timerDuration);

        isInitializing = false;
    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateGauge(timeRemaining, timerDuration);
        }
        else
        {
            timeRemaining = 0;
            UpdateGauge(timeRemaining, timerDuration);
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
        timeRemaining = timerDuration;
        InitializeGauge();
    }

    public void SetTimerDuration(float newDuration)
    {
        timerDuration = newDuration;
        ResetTimer();
    }
}