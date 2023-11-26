using UnityEngine;
using UnityEngine.InputSystem;
using LL.Input;
using System;
using LL.Game.Stats;
using UnityEngine.SceneManagement;

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
    float jumpForceMultiplier = 1f;
    [SerializeField]
    float diveSpeed;
    [SerializeField]
    float coyoteTime = 0.1f;
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
    float health;
    [SerializeField]
    float attackCooldown;
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
    LiveStatsBehavior stats;
    [SerializeField]
    StatResource moveSpeed;
    [SerializeField]
    StatResource climbing;
    [SerializeField]
    StatResource sneak;
    [SerializeField]
    StatResource vitality;
    [SerializeField]
    StatResource jumpForce;
    [SerializeField]
    StatResource ferocity;
    [SerializeField]
    StatResource airControl;
    private float groundTimer;

    private float jumpTimer;

    private float lastInputDirection;

    private bool running;

    MyPlayerInput input;

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
        groundCheck();

        if (!DisableInput)
        {
            if (input.Player.Jump.WasPressedThisFrame() && isGrounded())
            {
                var jumpImpulse = stats.GetValue(jumpForce) * jumpImpulseMultiplier;
                rigidBody.AddForce(jumpImpulse * Vector2.up, ForceMode2D.Impulse);
                OnJump?.Invoke(this, new EventArgs());
                jumpTimer = 0;
            }
            else if (input.Player.Jump.IsPressed())
            {
                var additionalJumpForce = stats.GetValue(jumpForce) * jumpForceMultiplier * Mathf.Pow(2, -jumpTimer * 5);
                rigidBody.AddForce(additionalJumpForce * Time.deltaTime / Time.fixedDeltaTime * Vector2.up, ForceMode2D.Force);
            }
            if (input.Player.Dive.WasPressedThisFrame())
            {
                rigidBody.AddForce(Vector2.down * diveSpeed, ForceMode2D.Impulse);
            }
            handleCombat();
            var moveDir = input.Player.Move.ReadValue<float>();
            if (Mathf.Abs(moveDir) > 0.05)
                lastInputDirection = moveDir;
            bool isShiftPressed = running = input.Player.Shift.ReadValue<float>() == 1;
            handleMovement(isShiftPressed, moveDir);
        }
        groundTimer += Time.deltaTime;
        jumpTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
    }

    private void groundCheck()
    {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, (boxCollider2d.bounds.size + 2 * boxCollider2d.edgeRadius * Vector3.one) * 0.995f, 0f, Vector2.down, extraHeight, platformLayerMask);
        if (raycastHit.collider != null)
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
        }
        else if (!isGrounded())
        {
            var airAcceleration = baseAirAcceleration * moveDir * stats.GetValue(airControl);
            rigidBody.AddForce(airAcceleration * Time.deltaTime / Time.fixedDeltaTime * Vector2.right);
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
                Collider2D[] damage = Physics2D.OverlapCircleAll(attackLocation.position, attackRange, enemies);
                foreach (Collider2D collision in damage)
                {
                    collision.gameObject.GetComponent<EnemyBehaviour>().takeDamage(attackDamage);
                }
            }
            currentAttackCooldown = attackCooldown;

        }
        else
        {
            currentAttackCooldown -= Time.deltaTime;
        }
    }

    public void takeDamage(float damage)
    {
        OnHit?.Invoke(this, new EventArgs());
        health -= damage;
    }

    private void restartGame() { }
}
