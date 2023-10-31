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
    public LayerMask platform;
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
        float extraDist = 0.01f;
        Vector3 size = new Vector2(boxCollider.bounds.size.x, boxCollider.bounds.size.y * 0.99f); // make the box cast slightly smaller on y-axis so it doesn't check for collissions on this axis
        RaycastHit2D leftCollision = Physics2D.BoxCast(boxCollider.bounds.center, size, 0f, Vector2.left, extraDist, platform);
        RaycastHit2D rightCollision = Physics2D.BoxCast(boxCollider.bounds.center, size, 0f, Vector2.right, extraDist, platform);
        if (leftCollision.collider != null)
        {
            Debug.Log("hello");
            direction = Vector2.right;
        }
        else if (rightCollision.collider != null) {
            Debug.Log("hello2");
            direction = Vector2.left;
        }

    }
}
