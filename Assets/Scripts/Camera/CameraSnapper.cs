using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraSnapper : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera cam;

    [SerializeField]
    LayerMask layerMask;

    [SerializeField]
    bool onlyShowOnce = false;

    public LayerMask LayerMask => layerMask;

    void Awake()
    {
        cam.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & layerMask) != 0)
        {
            cam.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & layerMask) != 0)
        {
            cam.enabled = false;
            if (onlyShowOnce)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
