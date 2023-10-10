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
    public float groundAcceleration;
    public float maxGroundSpeed;
    public float jumpSpeed;
    public float diveSpeed;
    public Rigidbody2D rigidBody;
    [SerializeField] private LayerMask platformLayerMask;
    public BoxCollider2D boxCollider2d;
    public AudioSource catWalk;
    public AudioSource catMeow;
    public AudioSource catRun;
    public AudioSource catAttack;
    public AudioSource catJump;
    public LiveStatsBehavior stats;
    public StatResource moveSpeed;
  

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
    }

    void Start() {
    }

    void Update() {
        if (input.Player.Jump.WasPressedThisFrame() && isGrounded()) {
            rigidBody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            catWalk.enabled = false;
        }
        if (input.Player.Dive.WasPressedThisFrame())
        {
            rigidBody.AddForce(Vector2.down * diveSpeed, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate() {
        var moveDir = input.Player.Move.ReadValue<float>();
        bool isShiftPressed = input.Player.Shift.ReadValue<float>() == 1;
        handleMovementSounds(isShiftPressed, moveDir);
        handleMovement(isShiftPressed, moveDir);
    }

    private bool isGrounded() {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, extraHeight, platformLayerMask);
        return raycastHit.collider != null;
    }

    private void handleMovement(bool isShiftPressed, float moveDir)
    {
        var sprintMultiplier = isShiftPressed && isGrounded() ? 1.3f : 1; // Maybe a variable or something
        var acceleration = Vector2.right * stats.GetValue(moveSpeed) * sprintMultiplier * moveDir * Time.deltaTime;
        if (rigidBody.velocity.x < maxGroundSpeed) {
            rigidBody.AddForce(acceleration);
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
}