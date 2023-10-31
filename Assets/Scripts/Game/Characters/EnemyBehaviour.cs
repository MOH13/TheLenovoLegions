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
    private bool dirRight;
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
        dirRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAttackCooldown <= 0) {
            Collider2D hit = Physics2D.OverlapCircle(attackLocation.position, attackRange, player); // fix so attackLocation corresponds to the way it is "looking"
            if (hit != null) {
                // More optimal way to do this rather than getting component every time?
                hit.gameObject.GetComponent<CatBehaviour>().takeDamage(damage);
                currentAttackCooldown = attackCooldown;
            }
        }
        else {
            currentAttackCooldown -= Time.deltaTime;
        }
        if (dirRight)
        {
            transform.Translate(Vector2.right * acceleration * Time.deltaTime);
        }
        else {
            transform.Translate(Vector2.left * acceleration * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    { // should be colission with a "wall type"
        if (false)
        dirRight = !dirRight;   
    }
}
