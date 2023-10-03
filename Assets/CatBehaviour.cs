using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using LL.Input;
using UnityEngine.Animations;
using UnityEngine.InputSystem.Composites;
using System;

public class CatBehaviour : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rigidBody;
    [SerializeField] private LayerMask platformLayerMask;
    public BoxCollider2D boxCollider2d;
    public AudioSource catWalk;
    public AudioSource catMeow;
    public AudioSource catRun;

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
    }

    void Start() {
        speed = 5;
    }

    void Update() {
        if (input.Player.Jump.WasPressedThisFrame() && isGrounded()) {
            rigidBody.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
            catWalk.enabled = false;
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
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size - new Vector3(0.1f, 0.1f, 0f), 0f, Vector2.down, extraHeight, platformLayerMask);
        return raycastHit.collider != null;
    }

    private void handleMovement(bool isShiftPressed, float moveDir)
    {
        if (input.Player.Dive.IsPressed()) {
            rigidBody.AddForce(Vector2.down * (speed / 10), ForceMode2D.Impulse);
        }
        var sprintMultiplier = isShiftPressed ? 1.3f : 1; // Maybe a variable or something
        rigidBody.AddForce(Vector2.right * speed * sprintMultiplier * moveDir * (Time.deltaTime * 60));
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
