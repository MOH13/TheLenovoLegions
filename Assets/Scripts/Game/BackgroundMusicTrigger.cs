using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicTrigger : MonoBehaviour
{
    [SerializeField]
    AudioSource? backgroundAmbience;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            SoundManager.instance.PlaySound(backgroundAmbience, 0.05f);
        }
    }
}
