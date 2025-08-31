using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource; 
    public AudioClip dayMusic;
    public AudioMixerGroup musicGroup;

    public void SetVolume(float value) 
    {
        musicGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    void Start()
    {
        audioSource.clip = dayMusic;
        audioSource.Play();
    }

  

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
