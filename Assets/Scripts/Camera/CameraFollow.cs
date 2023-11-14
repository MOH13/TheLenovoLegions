using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector2 offset;
    private Vector2 velocity = Vector2.zero;
    public float smoothTime;

    public float zValue = -10;

    void LateUpdate()
    {
        var targetPosition = (Vector2)target.position + offset;
        var smoothedPosition = Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, zValue);
    }
}
