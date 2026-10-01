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

    protected bool jumping;
    protected bool fallingAfterTheJump;

    public Action onJumped;
    public Action onJumpEnded;

    private void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (minJumpTime > 0f)
        {
            minJumpTime -= Time.deltaTime;
            if (minJumpTime <= 0f)
                OnMinJumpTime();
        }

        if(curJumpCooldown > 0f)
        {
            curJumpCooldown -= Time.deltaTime;
            if (curJumpCooldown <= 0f)
                OnJumpCooldown();
        }

        if (jumping && minJumpTime <= 0f && rigidbody.linearVelocityY <= 0f)
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

    protected virtual bool CanJump()
    {
        return movementManager.isGrounded && !jumping && curMinJumpTime <= 0f && curJumpCooldown <= 0f;
    }

    public void StopJump()
    {
        if (!jumping) return;
        if (rigidbody.linearVelocityY > 0f) rigidbody.linearVelocityY = rigidbody.linearVelocityY * stopJumpMultiplier;
        jumping = false;

        onJumpEnded?.Invoke();
    }

    protected virtual void OnGrounded()
    {
        if(fallingAfterTheJump) curJumpCooldown = jumpCooldown;
        fallingAfterTheJump = false;
    }

    protected virtual void OnEnable()
    {
        movementManager.onGrounded += OnGrounded;
    }

    protected virtual void OnDisable()
    {
        movementManager.onGrounded -= OnGrounded;
    }
}

