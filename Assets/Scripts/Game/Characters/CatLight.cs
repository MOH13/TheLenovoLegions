using System.Collections;
using System.Collections.Generic;
using LL.Game.Stats;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CatLight : MonoBehaviour
{
    [SerializeField]
    float baseInnerRadius = 0.03f;
    [SerializeField]
    float baseOuterRadius = 0.1f;
    [SerializeField]
    float baseIntensity = 0.1f;

    [SerializeField]
    LiveStatsBehavior stats;
    [SerializeField]
    StatResource nightVision;

    [SerializeField]
    Light2D catLight;


    void LateUpdate()
    {
        var visionStat = stats.GetValue(nightVision);
        catLight.pointLightInnerRadius = baseInnerRadius * visionStat;
        catLight.pointLightOuterRadius = baseOuterRadius * visionStat;
        catLight.intensity = baseIntensity * visionStat;
    }
}
