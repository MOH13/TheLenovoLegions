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
        if (collision.CompareTag("Ground")) {
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
        }
    }

    private void FixedUpdate() {
        if (input.Player.Dive.IsPressed()) {
            rigidBody.AddForce(Vector2.down * (speed / 10), ForceMode2D.Impulse);
        }
        var moveDir = input.Player.Move.ReadValue<float>();
        rigidBody.AddForce(Vector2.right * speed * moveDir * (Time.deltaTime * 60));
    }
}
