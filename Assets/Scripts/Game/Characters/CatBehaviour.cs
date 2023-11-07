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
    public event EventHandler? OnJump;

    [SerializeField]
    float baseAcceleration;
    [SerializeField]
    float baseSprintMultiplier;
    [SerializeField]
    float maxGroundSpeed;
    [SerializeField]
    float diveSpeed;
    [SerializeField]
    float coyoteTime = 0.1f;
    [SerializeField]
    float health;
    [SerializeField]
    float attackCooldown;
    [SerializeField]
    float currentAttackCooldown;
    [SerializeField]
    float attackRange;
    [SerializeField]
    float attackDamage;
    [SerializeField]
    Transform attackLocation;
    [SerializeField]
    Rigidbody2D rigidBody;
    [SerializeField]
    private LayerMask enemies;
    [SerializeField]
    private LayerMask platformLayerMask;
    [SerializeField]
    BoxCollider2D boxCollider2d;
    [SerializeField]
    LiveStatsBehavior stats;
    [SerializeField]
    StatResource moveSpeed;
    [SerializeField]
    StatResource climbing;
    [SerializeField]
    StatResource sneak;
    [SerializeField]
    StatResource vitality;
    [SerializeField]
    StatResource jumpForce;
    [SerializeField]
    StatResource ferocity;
    [SerializeField]
    StatResource nightVision;
    [SerializeField]
    StatResource airControl;
    private float groundTimer;

    private float lastInputDirection;

    private bool running;

    MyPlayerInput input;

    public float LastInputDirection => lastInputDirection;

    public float HorizontalSpeed => rigidBody.velocity.x;

    public float SneakValue => stats.GetValue(sneak);

    public bool Running => running;

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

    void Start()
    {
    }

    void Update()
    {
        groundCheck();
        if (input.Player.Jump.WasPressedThisFrame() && isGrounded())
        {
            rigidBody.AddForce(Vector2.up * stats.GetValue(jumpForce), ForceMode2D.Impulse);
            OnJump?.Invoke(this, new EventArgs());
        }
        if (input.Player.Dive.WasPressedThisFrame())
        {
            rigidBody.AddForce(Vector2.down * diveSpeed, ForceMode2D.Impulse);
        }
        handleCombat();
        var moveDir = input.Player.Move.ReadValue<float>();
        if (Mathf.Abs(moveDir) > 0.05)
            lastInputDirection = moveDir;
        bool isShiftPressed = running = input.Player.Shift.ReadValue<float>() == 1;
        handleMovement(isShiftPressed, moveDir);
        groundTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        //var moveDir = input.Player.Move.ReadValue<float>();
        //bool isShiftPressed = input.Player.Shift.ReadValue<float>() == 1;
        //handleMovementSounds(isShiftPressed, moveDir);
        //handleMovement(isShiftPressed, moveDir);
    }

    private void groundCheck()
    {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, extraHeight, platformLayerMask);
        if (raycastHit.collider != null)
        {
            groundTimer = 0;
        }
    }

    public bool isGrounded()
    {
        return groundTimer < coyoteTime;
    }

    private void handleMovement(bool isShiftPressed, float moveDir)
    {
        if (isGrounded() && HorizontalSpeed < maxGroundSpeed)
        {
            var sprintMultiplier = isShiftPressed && isGrounded() ? baseSprintMultiplier : 1; // Maybe a variable or something
            var groundAcceleration = baseAcceleration * moveDir * sprintMultiplier * stats.GetValue(moveSpeed) * Time.deltaTime * Vector2.right;
            rigidBody.AddForce(groundAcceleration);
        }
        else if (!isGrounded())
        {
            var airAcceleration = baseAcceleration * moveDir * stats.GetValue(airControl) * Time.deltaTime * Vector2.right;
            rigidBody.AddForce(airAcceleration);
        }
    }

    private void handleCombat()
    {
        // Needs animation
        if (health <= 0)
        {
            restartGame();
            Destroy(gameObject);
        }
        if (input.Player.Attack.WasPressedThisFrame())
        {
            if (currentAttackCooldown <= 0)
            {
                Collider2D[] damage = Physics2D.OverlapCircleAll(attackLocation.position, attackRange, enemies);
                foreach (Collider2D collision in damage)
                {
                    collision.gameObject.GetComponent<EnemyBehaviour>().takeDamage(attackDamage);
                }
            }
            currentAttackCooldown = attackCooldown;

        }
        else
        {
            currentAttackCooldown -= Time.deltaTime;
        }
    }

    public void takeDamage(float damage)
    {
        health -= damage;
    }

    private void restartGame() { }
}
