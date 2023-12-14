using System;
using UnityEngine;

[Serializable]
public struct SpriteConfig
{
    public float angle;
    public Sprite sprite;
}

public class SpiderVisuals : MonoBehaviour
{
    [SerializeField]
    private EnemyBehaviour enemyBehaviour;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private SpriteConfig[] spriteConfigs;

    void LateUpdate()
    {
        Sprite sprite = spriteRenderer.sprite;

        foreach (var config in spriteConfigs)
        {
            if (config.angle <= enemyBehaviour.CurrentSwingAngle)
            {
                sprite = config.sprite;
            }
            else break;
        }
        spriteRenderer.sprite = sprite;
        lineRenderer.SetPosition(0, enemyBehaviour.OriginalPosition);
        lineRenderer.SetPosition(1, transform.position);
    }
}
