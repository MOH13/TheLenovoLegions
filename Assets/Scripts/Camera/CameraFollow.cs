using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    float maxUpwardsOffset = 1f;

    [SerializeField]
    float maxDownwardsOffset = 1f;

    [SerializeField]
    float leadDistance = 1f;

    [SerializeField]
    float flipTime = 1f;

    [SerializeField]
    float zValue = -10;

    float camDir = 1;

    float targetPreviousXPos;

    void Start()
    {
        targetPreviousXPos = target.position.x;
    }

    void PositionCamera()
    {
        var x = target.position.x + camDir * leadDistance;
        var y = Mathf.Clamp(transform.position.y, target.position.y - maxDownwardsOffset, target.position.y + maxUpwardsOffset);

        transform.position = new Vector3(x, y, zValue);
    }

    void LateUpdate()
    {
        var xVelocity = (target.position.x - targetPreviousXPos) / Time.deltaTime;
        float dirChange = 0;
        if (xVelocity > 0.01f)
        {
            dirChange = 2 * Time.deltaTime / flipTime;
        }
        else if (xVelocity < -0.01f)
        {
            dirChange = -2 * Time.deltaTime / flipTime;
        }
        camDir = Mathf.Clamp(camDir + dirChange, -1, 1);
        targetPreviousXPos = target.position.x;

        PositionCamera();
    }
}
