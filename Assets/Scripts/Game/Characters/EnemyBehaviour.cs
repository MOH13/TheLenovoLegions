using System;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EnemyBehaviour : MonoBehaviour
{
    public float health;
    public float damage;
    public float attackRange;
    public float attackCooldown;
    public float currentAttackCooldown;
    public float acceleration;
    public bool shouldChase;
    public float chaseDistance;
    private Vector3 direction;
    public Transform attackLocation;
    public LayerMask player;
    public LayerMask collisionLayerMask;
    public Rigidbody2D rigidBody;
    public BoxCollider2D boxCollider;
    //[SerializeField] Animator transitionAnim;

    internal void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            //transitionAnim.SetTrigger("Death");
            //new WaitForSeconds(1);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        direction = Vector3.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAttackCooldown <= 0)
        {
            Collider2D hit = Physics2D.OverlapCircle(attackLocation.position, attackRange, player);
            if (hit != null)
            {
                var playerPosition = hit.gameObject.transform;
                Vector3 enemyDirection = playerPosition.position - transform.position;
                float angle = Vector2.Angle(direction, enemyDirection);
                if (angle < 30)
                {
                    hit.gameObject.GetComponent<CatBehaviour>().takeDamage(damage);
                    currentAttackCooldown = attackCooldown;
                }
            }
        }
        else
        {
            currentAttackCooldown -= Time.deltaTime;
        }
        if (shouldChase) {
            Collider2D playerCollision = Physics2D.OverlapCircle(transform.position, chaseDistance, player); // Maybe move into an OnCollision method
            if (playerCollision != null)
            {
                Vector3 aiPosition = transform.position, catPosition = playerCollision.transform.position;
                var cat = playerCollision.gameObject.GetComponent<CatBehaviour>(); // If cat is sneaking or standing still(?), we should consider sneak factor when chasing
                float distanceToPlayer = Vector2.Distance(aiPosition, catPosition);
                if (distanceToPlayer < chaseDistance)
                {
                    transform.position = Vector2.MoveTowards(aiPosition, catPosition, acceleration * Time.deltaTime);
                }
            }
            else
            {
                transform.Translate(direction * acceleration * Time.deltaTime);
            }
        } else {
            transform.Translate(direction * acceleration * Time.deltaTime);
        }
        float extraDist = 0.01f + boxCollider.bounds.extents.x * 0.05f;
        Vector3 size = new Vector2(boxCollider.bounds.size.x * 0.95f, boxCollider.bounds.size.y * 0.95f); // we make the box cast slightly smaller on y-axis so it doesn't check for collissions on this axis
        RaycastHit2D leftCollision = Physics2D.BoxCast(boxCollider.bounds.center, size, 0f, Vector2.left, extraDist, collisionLayerMask);
        RaycastHit2D rightCollision = Physics2D.BoxCast(boxCollider.bounds.center, size, 0f, Vector2.right, extraDist, collisionLayerMask);
        if (leftCollision.collider != null)
        {
            direction = Vector2.right;
        }
        else if (rightCollision.collider != null)
        {
            direction = Vector2.left;
        }

    }
}
