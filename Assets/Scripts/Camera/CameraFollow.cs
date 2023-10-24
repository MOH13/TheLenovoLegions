using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime;

    void LateUpdate()
    {
        var targetPosition = target.position + offset;
        var smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        transform.position = smoothedPosition;
    }
}
