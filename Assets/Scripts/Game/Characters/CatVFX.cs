using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatVFX : MonoBehaviour
{
    public const string JUMPED_KEY = "jumped";
    public const string AIRBORNE_KEY = "airborne";
    public const string HORIZONTAL_SPEED_KEY = "horizontal-speed";
    public const string FACING_LEFT_KEY = "facing-left";
    public const string WALL_CLIMBING_KEY = "climbing";
    public const string ATTACK_KEY = "attack";

    [SerializeField]
    CatBehaviour cat;

    [SerializeField]
    Animator animator;


    [SerializeField]
    AudioSource? walkSound;
    [SerializeField]
    AudioSource? meowSound;
    [SerializeField]
    AudioSource? attackSound;
    [SerializeField]
    AudioSource? jumpSound;
    [SerializeField]
    AudioSource? landSound;
    [SerializeField]
    AudioSource? hitSound;

    // Start is called before the first frame update
    void Start()
    {
        cat.OnJump += OnJump;
        cat.OnAttack += OnAttack;
        cat.OnHit += OnHit;
    }

    private void OnJump(object sender, EventArgs e)
    {
        animator.SetTrigger(JUMPED_KEY);
    }

    private void OnAttack(object sender, EventArgs e)
    {
        SoundManager.instance.PlaySound(attackSound, 0.1f);
        animator.SetTrigger(ATTACK_KEY);
    }

    private void OnHit(object sender, EventArgs e)
    {
        SoundManager.instance.PlaySound(hitSound, 0.05f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        animator.SetFloat(HORIZONTAL_SPEED_KEY, Mathf.Abs(cat.HorizontalSpeed));
        animator.SetBool(FACING_LEFT_KEY, cat.LastInputDirection < 0);
        animator.SetBool(AIRBORNE_KEY, !cat.isGrounded());
        animator.SetBool(WALL_CLIMBING_KEY, cat.WallClimbingDirection.HasValue);
    }

    void PlayMovementSound()
    {
        SoundManager.instance.PlaySound(walkSound, 0.1f);
    }

    void PlayJumpSound() {
        SoundManager.instance.PlaySound(jumpSound, 0.9f);
    }

    void PlayLandSound()
    {
        SoundManager.instance.PlaySound(landSound, 0.6f);
    }


}
