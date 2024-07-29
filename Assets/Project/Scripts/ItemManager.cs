using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [System.Serializable]
    public class Item
    {
        public string name;
        public Image buttonImage;
        public Button useButton;
        public float currentGauge;
        public float targetGauge;
        public float maxGauge = 100f;
        public Coroutine fillCoroutine;
    }

    public Item[] items = new Item[3];
    public float fillDuration = 0.5f; // 게이지가 차오르는 데 걸리는 시간

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
        for (int i = 0; i < items.Length; i++)
        {
            int index = i;
            items[i].useButton.onClick.AddListener(() => UseItem(index));
            UpdateGaugeUI(i);
        }
    }

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
            
            //아이템 효과 구현
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

    private void UpdateGaugeUI(int index)
    {
        float fillAmount = items[index].currentGauge / items[index].maxGauge;
        items[index].buttonImage.fillAmount = fillAmount;
        
        int percentage = Mathf.RoundToInt(fillAmount * 100);
        //items[index].percentageText.text = $"{percentage}%";

        items[index].useButton.interactable = percentage == 100;
    }
    
    private IEnumerator UseRedItemEffect()
    {
        CharacterManager.Instance.SetImage(4,0.35f);
        AudioManager.Instance.PlayItemsSounds(0);
        // 플레이어 입력 비활성화 
        
        InputManager.Instance.SetInputEnabled(false);
        //CharacterManager.Instance.SetImage(1,0.5f);
        yield return new WaitForSeconds(0.15f);
        //CharacterManager.Instance.SetImage(2,0.5f);
        yield return StartCoroutine(PieceManager.Instance.ClearAllAndDropNew());

        // 플레이어 입력 활성화 
        InputManager.Instance.SetInputEnabled(true);
        CharacterManager.Instance.SetImage(0,0.05f);
    }

    private IEnumerator UseBlueItemEffect()
    {
        CharacterManager.Instance.SetImage(4,0.35f);
        AudioManager.Instance.PlayItemsSounds(1);
        // 플레이어 입력 비활성화 
        InputManager.Instance.SetInputEnabled(false);
        //CharacterManager.Instance.SetImage(3,0.5f);
        yield return new WaitForSeconds(0.15f);
        yield return StartCoroutine(PieceManager.Instance.ClearCrossDrop());

        // 플레이어 입력 활성화 
        InputManager.Instance.SetInputEnabled(true);
        CharacterManager.Instance.SetImage(0,0.05f);
    }

    private void UsePinkItemEffect()
    {
        CharacterManager.Instance.SetImage(4,0.35f);
        AudioManager.Instance.PlayItemsSounds(2);
        GameManager.Instance.AddTime(10.0f);
        CharacterManager.Instance.SetImage(0,0.05f);
    }

}