using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioSource audioSource, float volume) {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    public void SetVolume(float volume) {
        AudioListener.volume = volume;
    }
}
