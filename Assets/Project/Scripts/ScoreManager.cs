using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    
    public static ScoreManager Instance { get; private set; }
    
    public TextMeshProUGUI scoreText;
    [SerializeField] int score = 0;


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

    public void AddScore(int points)
    {
        score += points;
        UpadteScoreUI();
    }


    private void UpadteScoreUI()
    {
        scoreText.text = "Score: " + score;
    }
}