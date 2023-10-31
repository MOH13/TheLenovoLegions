using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using LL.Input;
using UnityEngine.Animations;
using UnityEngine.InputSystem.Composites;
using System;
using LL.Framework.Stats;
using System.ComponentModel;
using LL.Game.Stats;

public class CatBehaviour : MonoBehaviour
{
    public float baseAcceleration;
    public float baseSprintMultiplier;
    public float maxGroundSpeed;
    public float diveSpeed;
    public float health;
    public float attackCooldown;
    public float currentAttackCooldown;
    public float attackRange;
    public Transform attackLocation;
    public Rigidbody2D rigidBody;
    [SerializeField] private LayerMask enemies;
    [SerializeField] private LayerMask platformLayerMask;
    public BoxCollider2D boxCollider2d;
    public AudioSource catWalk;
    public AudioSource catMeow;
    public AudioSource catRun;
    public AudioSource catAttack;
    public AudioSource catJump;
    public LiveStatsBehavior stats;
    public StatResource moveSpeed;
    public StatResource climbing;
    public StatResource sneak;
    public StatResource vitality;
    public StatResource jumpForce;
    public StatResource ferocity;
    public StatResource nightVision;
    public StatResource airControl;
  

    MyPlayerInput input;

    private void OnEnable()
    {
        input.Player.Enable();
        
    }
    private void OnDisable()
    {
        input.Player.Disable();
    }

    private void Awake()
    {
        input = new MyPlayerInput();
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        stats = GetComponent<LiveStatsBehavior>();
        health = stats.GetValue(vitality);
    }

    void Start() {
    }

    void Update() {
        if (input.Player.Jump.WasPressedThisFrame() && isGrounded()) {
            rigidBody.AddForce(Vector2.up * stats.GetValue(jumpForce), ForceMode2D.Impulse);
            catWalk.enabled = false;
        }
        if (input.Player.Dive.WasPressedThisFrame())
        {
            rigidBody.AddForce(Vector2.down * diveSpeed, ForceMode2D.Impulse);
        }
        handleCombat();
        var moveDir = input.Player.Move.ReadValue<float>();
        bool isShiftPressed = input.Player.Shift.ReadValue<float>() == 1;
        handleMovementSounds(isShiftPressed, moveDir);
        handleMovement(isShiftPressed, moveDir);
    }

    private void FixedUpdate() {
        //var moveDir = input.Player.Move.ReadValue<float>();
        //bool isShiftPressed = input.Player.Shift.ReadValue<float>() == 1;
        //handleMovementSounds(isShiftPressed, moveDir);
        //handleMovement(isShiftPressed, moveDir);
    }

    private bool isGrounded() {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, extraHeight, platformLayerMask);
        return raycastHit.collider != null;
    }

    private void handleMovement(bool isShiftPressed, float moveDir)
    {
        if (isGrounded() && rigidBody.velocity.magnitude < maxGroundSpeed) {
            var sprintMultiplier = isShiftPressed && isGrounded() ? baseSprintMultiplier : 1; // Maybe a variable or something
            var groundAcceleration = Vector2.right * stats.GetValue(moveSpeed) * baseAcceleration * sprintMultiplier * moveDir * Time.deltaTime;
            rigidBody.AddForce(groundAcceleration);
        }
        else if (!isGrounded()) {
            var airAcceleration = Vector2.right * stats.GetValue(airControl) * baseAcceleration * moveDir * Time.deltaTime;
            rigidBody.AddForce(airAcceleration);
        }
    }

    private void handleMovementSounds(bool isShiftPressed, float moveDir)
    {
        if (moveDir != 0 && isGrounded())
        {
            if (isShiftPressed)
            {
                catRun.enabled = true;
                catWalk.enabled = false;
            }
            else
            {
                catWalk.enabled = true;
                catRun.enabled = false;
            }
        }
        else
        {
            catWalk.enabled = false;
            catRun.enabled = false;
        }
    }

    private void handleCombat() {
        // Needs animation
        if (health <= 0) {
            restartGame();
        }
        if (input.Player.Attack.WasPressedThisFrame())
        {
            if (currentAttackCooldown <= 0)
            {
                Collider2D[] damage = Physics2D.OverlapCircleAll(attackLocation.position, attackRange, enemies);
                foreach (Collider2D collision in damage)
                {
                    Destroy(collision.gameObject); // change to reduce health instead
                }
            }
            currentAttackCooldown = attackCooldown;

        }
        else {
            currentAttackCooldown -= Time.deltaTime;
        }
    }

    private void restartGame() { }
}
