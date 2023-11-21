using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicTrigger : MonoBehaviour
{
    [SerializeField]
    AudioSource? backgroundAmbience;
    bool isPlaying;

    private void Awake()
    {
        isPlaying = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            if (!isPlaying)
            {
                backgroundAmbience.Play();
                isPlaying = true;
            }
        }
    }
}
