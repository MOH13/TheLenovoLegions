using System;
using UnityEngine;

[Serializable]
public struct SwingSetup
{
    public bool shouldSwing;
    public float swingDistance;
    public float swingPeriod;
    public float swingAmplitude;
}

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
    public SwingSetup swingSetup = new() { shouldSwing = false, swingDistance = 1, swingPeriod = 2, swingAmplitude = 120 };
    private Vector3 direction;

    public Vector2 OriginalPosition { get; private set; }
    public float CurrentSwingAngle { get; private set; }
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
        OriginalPosition = transform.position;
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
        if (swingSetup.shouldSwing)
        {
            CurrentSwingAngle = swingSetup.swingAmplitude * Mathf.Cos(Mathf.PI * 2 / swingSetup.swingPeriod * Time.time);
            float angle = (CurrentSwingAngle - 90) * Mathf.Deg2Rad;

            var x = OriginalPosition.x + swingSetup.swingDistance * Mathf.Cos(angle);
            var y = OriginalPosition.y + swingSetup.swingDistance * Mathf.Sin(angle);
            transform.position = new Vector2(x, y);
        }
        else if (shouldChase)
        {
            Collider2D playerCollision = Physics2D.OverlapCircle(transform.position, chaseDistance, player); // Maybe move into an OnCollision method
            if (playerCollision != null)
            {
                Vector3 aiPosition = transform.position, catPosition = playerCollision.gameObject.transform.position;
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
        }
        else
        {
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

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (swingSetup.shouldSwing)
        {
            Gizmos.DrawWireSphere(transform.position, swingSetup.swingDistance);
            var x = Mathf.Cos((swingSetup.swingAmplitude - 90) * Mathf.Deg2Rad);
            var y = Mathf.Sin((swingSetup.swingAmplitude - 90) * Mathf.Deg2Rad);
            Gizmos.DrawLine(transform.position, transform.position + swingSetup.swingDistance * new Vector3(x, y, 0));
            Gizmos.DrawLine(transform.position, transform.position + swingSetup.swingDistance * new Vector3(-x, y, 0));
        }
    }
#endif
}
