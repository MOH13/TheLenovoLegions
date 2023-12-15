using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorScript : MonoBehaviour
{
    [SerializeField]
    GameObject raccoon;
    Animator animator;
    EnemyBehaviour enemy;

    public const string FACING_LEFT_KEY = "facing-left";
    // Start is called before the first frame update

    private void Awake()
    {
        animator = raccoon.GetComponent<Animator>();
        enemy = raccoon.GetComponent<EnemyBehaviour>();
    }
    void LateUpdate()
    {
        animator.SetBool(FACING_LEFT_KEY, enemy.direction.x < 0);
    }
}
