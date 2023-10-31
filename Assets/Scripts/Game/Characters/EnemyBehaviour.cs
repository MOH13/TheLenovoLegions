using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyBehaviour : MonoBehaviour
{
    public float health;
    public float damage;
    public float attackRange;
    public float attackCooldown;
    public float currentAttackCooldown;
    public float acceleration;
    private Vector2 direction;
    public Transform attackLocation;
    public LayerMask player;
    public Rigidbody2D rigidBody;
    public BoxCollider2D boxCollider;

    internal void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        direction = Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAttackCooldown <= 0) {
            Collider2D hit = Physics2D.OverlapCircle(attackLocation.position, attackRange, player); // fix so attackLocation corresponds to the way it is "looking"
            if (hit != null) {
                hit.gameObject.GetComponent<CatBehaviour>().takeDamage(damage);
                currentAttackCooldown = attackCooldown;
            }
        }
        else {
            currentAttackCooldown -= Time.deltaTime;
        }
            transform.Translate(direction * acceleration * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision){
        if (transform.position.x < collision.GetContact(0).point.x)
        {
            Debug.Log("collision from right");
            direction = Vector2.left;
        }
        else {
            direction = Vector2.right;
            Debug.Log("collision from left");
        }
    }
}
