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
    private bool onGround;
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground")) { // Should improve this, so we only set onGround when cat is on ground and not just touching an object
            onGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground")) {
            onGround = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground")) {
            onGround = true;
        }
    }

    // Start is called before the first frame update
    void Start() {
        speed = 5;
    }


    // Update is called once per frame
    void Update() {
        if (input.Player.Jump.WasPressedThisFrame() && onGround) {
            rigidBody.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
            catWalk.enabled = false;
        }
    }

    private void FixedUpdate() {
        if (input.Player.Dive.IsPressed()) {
            rigidBody.AddForce(Vector2.down * (speed / 10), ForceMode2D.Impulse);
        }
        var moveDir = input.Player.Move.ReadValue<float>();
        bool isShiftPressed = input.Player.Shift.ReadValue<float>() == 1;
        if (moveDir != 0 && onGround)
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
        var sprintMultiplier = isShiftPressed ? 1.3f : 1;
        rigidBody.AddForce(Vector2.right * speed * sprintMultiplier * moveDir * (Time.deltaTime * 60));
    }
}
