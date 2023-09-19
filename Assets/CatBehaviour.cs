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

    MyPlayerInput input;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        speed = 5;
        input = new MyPlayerInput();
        input.Enable();
    }


    // Update is called once per frame
    void Update()
    {
        if (input.Player.Jump.WasPressedThisFrame())
        {
            rigidBody.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        var moveDir = input.Player.Move.ReadValue<float>();
        rigidBody.AddForce(Vector2.right * speed * moveDir * (Time.deltaTime * 60));
    }
}
