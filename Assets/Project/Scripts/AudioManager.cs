using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    public static AudioManager Instance {get; private set; }
    public AudioSource effectsSource;
    public AudioSource voiceSource;
    
    public AudioClip[] comboEffects;
    public AudioClip undoSound;

    public AudioClip selectSound;

    public AudioClip[] comboVoices;
    public AudioClip swapSound;
    
    public AudioClip ItemButtonSound;
    public AudioClip[] ItemsSounds;
    public AudioClip[] ItemsVoices;
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
    
    public void PlayItemsSounds(int itemNumber)
    {
        if (itemNumber < ItemsSounds.Length && itemNumber < ItemsVoices.Length)
        {
            PlaySoundEffect(ItemsSounds[itemNumber]);
            PlayVoice(ItemsVoices[itemNumber]);
        }
        else
        {
            Debug.Log("itemNumber too Long");
        }
        
    }

    public void PlayButtonSound()
    {
        PlaySoundEffect(ItemButtonSound);
    }

    public void PlaySelectSound()
    {
        PlaySoundEffect(selectSound);
    }
    
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

    public void PlaySwapSound()
    {
        PlaySoundEffect(swapSound);
    }
    public void PlayUndoSound()
    {
        PlaySoundEffect(undoSound);
    }
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

    private void PlayVoice(AudioClip clip)
    {
        if(clip != null) voiceSource?.PlayOneShot(clip);
    }
    private void PlaySoundEffect(AudioClip clip)
    {
        if(clip != null) effectsSource?.PlayOneShot(clip);
    }
}
