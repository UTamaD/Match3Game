using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 게임 점수 관리를 담당하는 클래스
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ScoreManager Instance { get; private set; }
    
    public TextMeshProUGUI scoreText;    // 점수 표시 UI 텍스트
    [SerializeField] int score = 0;      // 현재 점수

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
    /// 점수 추가 함수
    /// </summary>
    /// <param name="points">추가할 점수</param>
    public void AddScore(int points)
    {
        score += points;
        UpadteScoreUI();
    }

    /// <summary>
    /// 점수 UI 업데이트 함수
    /// </summary>
    private void UpadteScoreUI()
    {
        scoreText.text = "Score: " + score;
    }
}