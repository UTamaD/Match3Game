using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 오디오 효과를 관리하는 클래스
/// </summary>
public class AudioManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static AudioManager Instance {get; private set; }
    
    public AudioSource effectsSource;    // 효과음 오디오 소스
    public AudioSource voiceSource;      // 음성 오디오 소스
    
    public AudioClip[] comboEffects;     // 콤보 효과음 배열
    public AudioClip undoSound;          // 취소 효과음
    public AudioClip selectSound;        // 선택 효과음
    public AudioClip[] comboVoices;      // 콤보 음성 배열
    public AudioClip swapSound;          // 교환 효과음
    
    public AudioClip ItemButtonSound;    // 아이템 버튼 효과음
    public AudioClip[] ItemsSounds;      // 아이템 효과음 배열
    public AudioClip[] ItemsVoices;      // 아이템 음성 배열

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
    /// 아이템 효과음 재생 함수
    /// </summary>
    /// <param name="itemNumber">아이템 번호</param>
    public void PlayItemsSounds(int itemNumber)
    {
        if (itemNumber < ItemsSounds.Length && itemNumber < ItemsVoices.Length)
        {
            PlaySoundEffect(ItemsSounds[itemNumber]);
            PlayVoice(ItemsVoices[itemNumber]);
        }
        else
        {
            Debug.Log("아이템 번호가 너무 큽니다");
        }
    }

    /// <summary>
    /// 버튼 효과음 재생 함수
    /// </summary>
    public void PlayButtonSound()
    {
        PlaySoundEffect(ItemButtonSound);
    }

    /// <summary>
    /// 선택 효과음 재생 함수
    /// </summary>
    public void PlaySelectSound()
    {
        PlaySoundEffect(selectSound);
    }
    
    /// <summary>
    /// 최대 콤보 효과음 재생 함수
    /// </summary>
    /// <param name="curruntCombo">현재 콤보 수</param>
    public void PlayMaxComboSound(int curruntCombo)
    {
        if (curruntCombo < comboVoices.Length)
        {
            PlayVoice(comboVoices[curruntCombo]);
        }
        else
        {
            PlayVoice(comboVoices[2]);
        }
    }

    /// <summary>
    /// 조각 교환 효과음 재생 함수
    /// </summary>
    public void PlaySwapSound()
    {
        PlaySoundEffect(swapSound);
    }

    /// <summary>
    /// 취소 효과음 재생 함수
    /// </summary>
    public void PlayUndoSound()
    {
        PlaySoundEffect(undoSound);
    }

    /// <summary>
    /// 콤보 효과음 재생 함수
    /// </summary>
    /// <param name="currentCombo">현재 콤보 수</param>
    public void PlayComboSound(int currentCombo)
    {
        if (comboEffects.Length > currentCombo)
        {
            PlaySoundEffect(comboEffects[currentCombo]);
        }
        else
        {
            PlaySoundEffect(comboEffects[2]);
        }
    }

    /// <summary>
    /// 음성 클립 재생 함수
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    private void PlayVoice(AudioClip clip)
    {
        if(clip != null) voiceSource?.PlayOneShot(clip);
    }

    /// <summary>
    /// 효과음 클립 재생 함수
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    private void PlaySoundEffect(AudioClip clip)
    {
        if(clip != null) effectsSource?.PlayOneShot(clip);
    }
}
