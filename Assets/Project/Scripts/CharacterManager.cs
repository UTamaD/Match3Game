using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 캐릭터 이미지를 관리하는 클래스
/// </summary>
public class CharacterManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static CharacterManager Instance { get; private set; }
    
    public GameObject[] characterList;       // 캐릭터 이미지 리스트

    private int currentImageNum = 0;         // 현재 활성화된 이미지 번호
    private Coroutine imageCoroutine = null; // 이미지 전환 코루틴

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
    /// 캐릭터 이미지 설정 함수
    /// </summary>
    /// <param name="number">표시할 이미지 번호</param>
    /// <param name="duration">표시 지속 시간</param>
    public void SetImage(int number, float duration)
    {
        if (imageCoroutine == null)
        {
            imageCoroutine = StartCoroutine(SetImageCoroutine(number, duration));
        }
    }
    
    /// <summary>
    /// 캐릭터 이미지 전환 코루틴
    /// </summary>
    /// <param name="number">표시할 이미지 번호</param>
    /// <param name="duration">표시 지속 시간</param>
    /// <returns>대기 시간</returns>
    private IEnumerator SetImageCoroutine(int number, float duration)
    {
        if (number < characterList.Length)
        {
            characterList[currentImageNum].SetActive(false);
            currentImageNum = number;
            characterList[currentImageNum].SetActive(true);
        }
        yield return new WaitForSeconds(duration);
        imageCoroutine = null;
    }
}
