using UnityEngine;
using UnityEngine.InputSystem;
using LL.Input;
using System;
using LL.Game.Stats;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEditor.PackageManager;

public class CatBehaviour : MonoBehaviour
{
    public event EventHandler? OnJump;
    public event EventHandler? OnAttack;
    public event EventHandler? OnHit;

    [SerializeField]
    float baseAcceleration;
    [SerializeField]
    float baseAirAcceleration = 0.5f;
    [SerializeField]
    float baseSprintMultiplier;
    [SerializeField]
    float maxGroundSpeedMultiplier;
    [SerializeField]
    float jumpImpulseMultiplier = 0.5f;
    [SerializeField]
    float wallJumpImpulseMultiplier = 0.3f;
    [SerializeField]
    float jumpForceMultiplier = 1f;
    [SerializeField]
    float diveSpeed;
    [SerializeField]
    float coyoteTime = 0.1f;
    [SerializeField]
    float baseClimbingDuration = 2f;
    [SerializeField]
    float stepCheckHeightLower = 0.01f;
    [SerializeField]
    float stepCheckHeightUpper = 0.1f;
    [SerializeField]
    float stepCheckDistance = 0.05f;
    [SerializeField]
    float stepSnapHeight = 0.1f;
    [SerializeField]
    float stepSnapXOffset = 0.03f;
    [SerializeField]
    public float health;
    [SerializeField]
    float attackCooldown;
    [SerializeField]
    float enemyBounceVelocity = 0.5f;
    [SerializeField]
    float currentAttackCooldown;
    [SerializeField]
    float attackRange;
    [SerializeField]
    float attackDamage;
    [SerializeField]
    Transform attackLocation;
    [SerializeField]
    Rigidbody2D rigidBody;
    [SerializeField]
    PlayerInput playerInput;
    [SerializeField]
    private LayerMask enemies;
    [SerializeField]
    private LayerMask platformLayerMask;
    [SerializeField]
    BoxCollider2D boxCollider2d;
    [SerializeField]
    public LiveStatsBehavior stats;
    [SerializeField]
    StatResource moveSpeed;
    [SerializeField]
    StatResource climbing;
    [SerializeField]
    StatResource sneak;
    [SerializeField]
    public StatResource vitality;
    [SerializeField]
    StatResource jumpForce;
    [SerializeField]
    StatResource ferocity;
    [SerializeField]
    StatResource airControl;
    private float groundTimer;

    private float jumpTimer;

    private Vector2? wallClimbingDirection;
    private float remainingClimbStrength;

    private float lastInputDirection;

    private bool running;

    MyPlayerInput input;

    public Vector2? WallClimbingDirection => wallClimbingDirection;

    public float LastInputDirection => lastInputDirection;

    public float HorizontalSpeed => rigidBody.velocity.x;

    public float SneakValue => stats.GetValue(sneak);

    public bool Running => running;

    public bool DisableInput { get; set; } = false;

    private void OnEnable()
    {
        if (input == null)
            input = new MyPlayerInput();
        input.Player.Enable();

    }
    private void OnDisable()
    {
        input.Player.Disable();
    }

    private void Awake()
    {
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        stats = GetComponent<LiveStatsBehavior>();
        health = stats.GetValue(vitality);
    }

    void Update()
    {
        if (!DisableInput)
        {
            if (input.Player.Jump.WasPressedThisFrame() && (isGrounded() || wallClimbingDirection != null))
            {
                var jumpImpulse =
                    isGrounded()
                    ? stats.GetValue(jumpForce) * jumpImpulseMultiplier * Vector2.up
                    : stats.GetValue(climbing) * wallJumpImpulseMultiplier * (Vector2.up - wallClimbingDirection!.Value * 0.5f);
                if (isGrounded())
                    rigidBody.AddForce(jumpImpulse, ForceMode2D.Impulse);
                else
                    rigidBody.velocity = jumpImpulse;
                OnJump?.Invoke(this, new());
                jumpTimer = 0;
                remainingClimbStrength = 1;
            }
            if (input.Player.Dive.WasPressedThisFrame())
            {
                rigidBody.AddForce(Vector2.down * diveSpeed, ForceMode2D.Impulse);
            }
        }
        jumpTimer += Time.deltaTime;
        handleCombat();
    }

    private void FixedUpdate()
    {
        groundCheck();

        if (!DisableInput)
        {
            if (input.Player.Jump.IsPressed())
            {
                var additionalJumpForce = stats.GetValue(jumpForce) * jumpForceMultiplier * Mathf.Pow(2, -jumpTimer * 5);
                rigidBody.AddForce(additionalJumpForce * Time.deltaTime / Time.fixedDeltaTime * Vector2.up, ForceMode2D.Force);
            }
            var moveDir = input.Player.Move.ReadValue<float>();
            if (Mathf.Abs(moveDir) > 0.05)
                lastInputDirection = moveDir;
            bool isShiftPressed = running = input.Player.Shift.ReadValue<float>() == 1;
            handleMovement(isShiftPressed, moveDir);
            groundTimer += Time.deltaTime;
        }
    }

    private void groundCheck()
    {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, (boxCollider2d.bounds.size + 2 * boxCollider2d.edgeRadius * Vector3.one) * 0.95f, 0f, Vector2.down, extraHeight, platformLayerMask);
        if (raycastHit.collider != null && Vector2.Dot(Vector2.up, raycastHit.normal) > 0.5f)
        {
            groundTimer = 0;
        }
    }

    public bool isGrounded()
    {
        return groundTimer < coyoteTime;
    }

    private void handleMovement(bool isShiftPressed, float moveDir)
    {
        wallClimbingDirection = null;

        var moveSpeedStat = stats.GetValue(moveSpeed);
        var sprintMultiplier = isShiftPressed && isGrounded() ? baseSprintMultiplier : 1;
        if (isGrounded())
        {
            handleStepSnap(moveDir);
            if (Mathf.Abs(HorizontalSpeed) < moveSpeedStat * sprintMultiplier * maxGroundSpeedMultiplier)
            {
                var groundAcceleration = baseAcceleration * moveDir * sprintMultiplier * moveSpeedStat;
                rigidBody.AddForce(groundAcceleration * Time.deltaTime / Time.fixedDeltaTime * Vector2.right);
            }
            remainingClimbStrength = 1;
        }
        else if (!isGrounded())
        {
            RaycastHit2D hit;
            if (remainingClimbStrength > 0 && Mathf.Abs(moveDir) > 0.1f && (hit = Physics2D.Raycast(boxCollider2d.bounds.center, moveDir > 0 ? Vector2.right : Vector2.left, 0.1f + boxCollider2d.bounds.extents.x + boxCollider2d.edgeRadius, platformLayerMask)))
            {

                var climbingStat = stats.GetValue(climbing);

                wallClimbingDirection = Mathf.Sign(moveDir) * Vector2.right;

                if (Mathf.Abs(rigidBody.velocity.x) < 0.05f)
                {
                    transform.position = new Vector2(hit.point.x - wallClimbingDirection.Value.x * (boxCollider2d.bounds.extents.x + boxCollider2d.edgeRadius) + Physics2D.defaultContactOffset, transform.position.y);
                }

                var falling = rigidBody.velocity.y < 0;
                float multiplier = 1;
                if (remainingClimbStrength > 0.90f)
                {
                    multiplier = Mathf.Lerp(0.5f, 1, (1 - remainingClimbStrength) * 10);
                }
                else if (remainingClimbStrength < 0.2f)
                {
                    multiplier = Mathf.Lerp(1, 0.5f, (0.2f - remainingClimbStrength) * 5);
                }
                var force = falling ? 5 - Physics2D.gravity.y : -5;
                rigidBody.AddForce(multiplier * force * Time.deltaTime * Vector2.up / Time.fixedDeltaTime);
                remainingClimbStrength -= Time.deltaTime / (baseClimbingDuration * climbingStat);
            }
            else
            {
                var airAcceleration = baseAirAcceleration * moveDir * stats.GetValue(airControl);
                rigidBody.AddForce(airAcceleration * Time.deltaTime / Time.fixedDeltaTime * Vector2.right);
            }
        }
    }

    private void handleStepSnap(float moveDir)
    {
        if (Mathf.Abs(moveDir) < 0.05f)
            return;

        var checkDir = moveDir > 0 ? Vector2.right : Vector2.left;
        var bounds = boxCollider2d.bounds;
        var start = new Vector2(bounds.center.x + (bounds.extents.x + boxCollider2d.edgeRadius) * Mathf.Sign(moveDir), bounds.min.y - boxCollider2d.edgeRadius);

        if (Physics2D.Raycast(start + Vector2.up * stepCheckHeightLower, checkDir, stepCheckDistance, platformLayerMask).collider != null)
        {
            // Lower check is occluded
            if (Physics2D.Raycast(start + Vector2.up * stepCheckHeightUpper, checkDir, stepCheckDistance, platformLayerMask).collider == null)
            {
                // Upper check is not occluded
                transform.position = transform.position + new Vector3(Mathf.Sign(moveDir) * stepSnapXOffset, stepSnapHeight, 0);
            }
        }
    }

    private void handleCombat()
    {
        if (health <= 0)
        {
            SceneController.Instance.NextLevel(SceneManager.GetActiveScene().name);
        }
        if (input.Player.Attack.WasPressedThisFrame())
        {
            if (currentAttackCooldown <= 0)
            {
                OnAttack?.Invoke(this, new EventArgs());
                Vector2 playerDirection = lastInputDirection > 0 ? Vector2.right : Vector2.left;
                Collider2D[] damage = Physics2D.OverlapCircleAll(attackLocation.position, attackRange, enemies);
                foreach (Collider2D collision in damage)
                {
                    var enemyPosition = collision.gameObject.transform;
                    Vector3 enemyDirection = enemyPosition.position - transform.position;
                    float angle = Vector3.Angle(playerDirection, enemyDirection);
                    if (angle <= 30)
                    {
                        collision.gameObject.GetComponent<EnemyBehaviour>().takeDamage(attackDamage);
                    }
                }

            }
            currentAttackCooldown = attackCooldown;

        }
        else
        {
            currentAttackCooldown -= Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (((1 << col.gameObject.layer) & enemies) != 0)
        {
            if (col.contacts[0].point.y < boxCollider2d.bounds.min.y)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, enemyBounceVelocity);
                OnAttack?.Invoke(this, new());
                col.gameObject.GetComponent<EnemyBehaviour>().takeDamage(attackDamage);
            }
        }
    }

    public void takeDamage(float damage)
    {
<<<<<<< HEAD
=======
        OnHit?.Invoke(this, new());
>>>>>>> main
        health -= damage;
        OnHit?.Invoke(this, new EventArgs());
    }
}
