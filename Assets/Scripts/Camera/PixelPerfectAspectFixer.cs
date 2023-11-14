using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PixelPerfectAspectFixer : MonoBehaviour
{
    [SerializeField]
    PixelPerfectCamera ppCam;

    [SerializeField]
    Camera cam;

    void Update()
    {
        ppCam.refResolutionX = Mathf.CeilToInt(ppCam.refResolutionY * Screen.width / (float)Screen.height);
    }
}
