using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    public string nextLevel;
    public string checkpointGameObject;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (string.IsNullOrEmpty(checkpointGameObject))
                SceneController.Instance.NextLevel(nextLevel);
            else
                SceneController.Instance.NextLevel(nextLevel, checkpointGameObject);
        }
    }
}
