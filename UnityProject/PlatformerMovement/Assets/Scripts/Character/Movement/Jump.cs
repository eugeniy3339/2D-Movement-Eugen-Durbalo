using System;
using UnityEngine;

public class Jump : MonoBehaviour
{
    protected MovementManager movementManager;
    protected Rigidbody2D rigidbody;

    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float minJumpTime = 0.1f;
    protected float curMinJumpTime;

    [SerializeField] private float jumpCooldown = 0f;
    protected float curJumpCooldown;

    [SerializeField] private float stopJumpMultiplier = 0.5f;

    [SerializeField] private float wallJumpMinJumpTime = 0.1f;
    [SerializeField, Tooltip("Wall jump right direction")] private Vector2 wallJumpDirection = Vector2.one;
    [SerializeField] private float wallJumpForce = 10f;

    protected bool jumping;
    protected bool fallingAfterTheJump;

    public event Action onJumped;
    public event Action<Vector2> onWallJumped;
    public event Action onJumpStopped;

    private void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (curMinJumpTime > 0f)
        {
            curMinJumpTime -= Time.deltaTime;
            if (curMinJumpTime <= 0f)
                OnMinJumpTime();
        }

        if(curJumpCooldown > 0f)
        {
            curJumpCooldown -= Time.deltaTime;
            if (curJumpCooldown <= 0f)
                OnJumpCooldown();
        }

        if (jumping && curMinJumpTime <= 0f && rigidbody.linearVelocityY <= 0f)
            StopJump();
    }

    protected virtual void OnMinJumpTime() { }

    protected virtual void OnJumpCooldown() { }

    public bool JumpIfCanTo()
    {
        if (CanJump())
        {
            jump();
            return true;
        }
        else if(CanWallJump())
        {
            WallJump();
            return true;
        }

        return false;
    }

    private void jump()
    {
        jumping = true;
        fallingAfterTheJump = true;
        curMinJumpTime = minJumpTime;
        rigidbody.linearVelocityY = jumpForce;

        onJumped?.Invoke();
    }

    private void WallJump()
    {
        float xMultiplier = movementManager.curWall != null ? NormalizedFloat.NormalizeFloat(transform.position.x - movementManager.curWall.transform.position.x) : 1f;
        Vector2 jumpDir = new Vector2(wallJumpDirection.x * xMultiplier, wallJumpDirection.y).normalized;

        jumping = true;
        fallingAfterTheJump = true;
        curMinJumpTime = wallJumpMinJumpTime;
        rigidbody.linearVelocity = jumpDir * wallJumpForce;

        onWallJumped?.Invoke(jumpDir);
    }

    protected virtual bool CanJump()
    {
        return movementManager.isGrounded && movementManager.movementState == MovementManager.MovementState.Default && !jumping && curMinJumpTime <= 0f && curJumpCooldown <= 0f;
    }

    protected virtual bool CanWallJump()
    {
        return movementManager.movementState == MovementManager.MovementState.OnWall && movementManager.curWall != null && !jumping && curMinJumpTime <= 0f && curJumpCooldown <= 0f;
    }

    public void StopJump()
    {
        if (!jumping) return;
        if (rigidbody.linearVelocityY > 0f) rigidbody.linearVelocityY = rigidbody.linearVelocityY * stopJumpMultiplier;
        jumping = false;

        onJumpStopped?.Invoke();
    }

    protected virtual void OnGrounded()
    {
        OnLanded();
    }

    protected virtual void OnGotOnTheWall(Collider2D wall)
    {
        OnLanded();
    }

    private void OnLanded()
    {
        if (fallingAfterTheJump) curJumpCooldown = jumpCooldown;
        fallingAfterTheJump = false;
    }

    protected virtual void OnEnable()
    {
        movementManager.onGrounded += OnGrounded;
        movementManager.onGotOnTheWall += OnGotOnTheWall;
    }

    protected virtual void OnDisable()
    {
        movementManager.onGrounded -= OnGrounded;
        movementManager.onGotOnTheWall -= OnGotOnTheWall;
    }
}

