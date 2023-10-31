using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float health;
    public float damage;
    public float attackRange;
    public float attackCooldown;
    public float currentAttackCooldown;
    public Transform attackLocation;
    [SerializeField] private LayerMask player;

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
    }
}
