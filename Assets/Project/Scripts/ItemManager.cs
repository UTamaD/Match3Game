using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 게임 아이템 관리를 담당하는 클래스
/// </summary>
public class ItemManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ItemManager Instance { get; private set; }

    /// <summary>
    /// 게임 내 아이템 정보를 담는 클래스
    /// </summary>
    [System.Serializable]
    public class Item
    {
        public string name;           // 아이템 이름
        public Image buttonImage;     // 아이템 버튼 이미지
        public Button useButton;      // 아이템 사용 버튼
        public float currentGauge;    // 현재 게이지 값
        public float targetGauge;     // 목표 게이지 값
        public float maxGauge = 100f; // 최대 게이지 값
        public Coroutine fillCoroutine; // 게이지 채우기 코루틴
    }

    public Item[] items = new Item[3];        // 아이템 배열
    public float fillDuration = 0.5f;         // 게이지가 차오르는 데 걸리는 시간

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
        for (int i = 0; i < items.Length; i++)
        {
            int index = i;
            items[i].useButton.onClick.AddListener(() => UseItem(index));
            UpdateGaugeUI(i);
        }
    }

    /// <summary>
    /// 아이템 게이지 증가 함수
    /// </summary>
    /// <param name="tag">아이템 태그</param>
    /// <param name="amount">증가량</param>
    public void AddGauge(string tag, float amount)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].name == tag)
            {
                items[i].targetGauge = Mathf.Min(items[i].currentGauge + amount, items[i].maxGauge);
                
                // 이미 실행 중인 코루틴이 있다면 중지
                if (items[i].fillCoroutine != null)
                {
                    StopCoroutine(items[i].fillCoroutine);
                }
                
                // 새 코루틴 시작
                items[i].fillCoroutine = StartCoroutine(FillGaugeSmooth(i));
                break;
            }
        }
    }

    /// <summary>
    /// 게이지를 부드럽게 채우는 코루틴
    /// </summary>
    /// <param name="index">아이템 인덱스</param>
    /// <returns>대기 시간</returns>
    private IEnumerator FillGaugeSmooth(int index)
    {
        float elapsedTime = 0f;
        float startGauge = items[index].currentGauge;
        float endGauge = items[index].targetGauge;

        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fillDuration;
            items[index].currentGauge = Mathf.Lerp(startGauge, endGauge, t);
            UpdateGaugeUI(index);
            yield return null;
        }

        items[index].currentGauge = endGauge;
        UpdateGaugeUI(index);
        items[index].fillCoroutine = null;
    }

    /// <summary>
    /// 아이템 사용 함수
    /// </summary>
    /// <param name="index">사용할 아이템 인덱스</param>
    private void UseItem(int index)
    {
        if (items[index].currentGauge >= items[index].maxGauge && !PieceManager.Instance.IsPieceMoving)
        {
            Debug.Log($"Using item: {items[index].name}");
            
            AudioManager.Instance.PlayButtonSound();
            
            items[index].targetGauge = 0f;
            
            if (items[index].fillCoroutine != null)
            {
                StopCoroutine(items[index].fillCoroutine);
            }
            
            items[index].fillCoroutine = StartCoroutine(FillGaugeSmooth(index));
            
            // 아이템 효과 구현
            switch (items[index].name)
            {
                case "Red":
                    StartCoroutine(UseRedItemEffect());
                    break;
                case "Blue":
                    StartCoroutine(UseBlueItemEffect());
                    break;
                case "Pink":
                    UsePinkItemEffect();
                    break;
            }
        }
    }

    /// <summary>
    /// 게이지 UI 업데이트 함수
    /// </summary>
    /// <param name="index">아이템 인덱스</param>
    private void UpdateGaugeUI(int index)
    {
        float fillAmount = items[index].currentGauge / items[index].maxGauge;
        items[index].buttonImage.fillAmount = fillAmount;
        
        int percentage = Mathf.RoundToInt(fillAmount * 100);
        //items[index].percentageText.text = $"{percentage}%";

        items[index].useButton.interactable = percentage == 100;
    }
    
    /// <summary>
    /// 빨간색 아이템 효과 실행 코루틴 (모든 조각 지우기)
    /// </summary>
    /// <returns>대기 시간</returns>
    private IEnumerator UseRedItemEffect()
    {
        CharacterManager.Instance.SetImage(4, 0.35f);
        AudioManager.Instance.PlayItemsSounds(0);
        // 플레이어 입력 비활성화 
        
        InputManager.Instance.SetInputEnabled(false);
        yield return new WaitForSeconds(0.15f);
        yield return StartCoroutine(PieceManager.Instance.ClearAllAndDropNew());

        // 플레이어 입력 활성화 
        InputManager.Instance.SetInputEnabled(true);
        CharacterManager.Instance.SetImage(0, 0.05f);
    }

    /// <summary>
    /// 파란색 아이템 효과 실행 코루틴 (십자 모양으로 조각 지우기)
    /// </summary>
    /// <returns>대기 시간</returns>
    private IEnumerator UseBlueItemEffect()
    {
        CharacterManager.Instance.SetImage(4, 0.35f);
        AudioManager.Instance.PlayItemsSounds(1);
        // 플레이어 입력 비활성화 
        InputManager.Instance.SetInputEnabled(false);
        yield return new WaitForSeconds(0.15f);
        yield return StartCoroutine(PieceManager.Instance.ClearCrossDrop());

        // 플레이어 입력 활성화 
        InputManager.Instance.SetInputEnabled(true);
        CharacterManager.Instance.SetImage(0, 0.05f);
    }

    /// <summary>
    /// 분홍색 아이템 효과 실행 함수 (시간 추가)
    /// </summary>
    private void UsePinkItemEffect()
    {
        CharacterManager.Instance.SetImage(4, 0.35f);
        AudioManager.Instance.PlayItemsSounds(2);
        GameManager.Instance.AddTime(10.0f);
        CharacterManager.Instance.SetImage(0, 0.05f);
    }
}