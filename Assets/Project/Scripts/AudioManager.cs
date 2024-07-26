using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    public static AudioManager Instance {get; private set; }
    public AudioSource effectsSource;
    public AudioClip[] comboEffects;
    public AudioClip undoSound;

    public AudioClip selectSound;
    public AudioClip maxComboVoice;
    public AudioClip swapSound;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlaySelectSoubd()
    {
        PlaySoundEffect(selectSound);
    }
    
    public void PlayMaxComboSound()
    {
        
        PlaySoundEffect(maxComboVoice);
        
    }

    public void PlaySwapSound()
    {
        PlaySoundEffect(swapSound);
    }
    public void PlayUndoSound()
    {
        PlaySoundEffect(undoSound);
    }
    public void playComboSound(int currentCombo)
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

    private void PlaySoundEffect(AudioClip clip)
    {
        if(clip != null) effectsSource.PlayOneShot(clip);
    }
}
