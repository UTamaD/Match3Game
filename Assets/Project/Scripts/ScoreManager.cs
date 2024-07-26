using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    
    public static ScoreManager Instance { get; private set; }
    
    public TextMeshProUGUI scoreText;
    private int score;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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