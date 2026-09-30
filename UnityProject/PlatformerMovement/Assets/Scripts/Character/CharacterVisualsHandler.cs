using UnityEngine;

public class CharacterVisualsHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private MovementManager movementManager;
    private Movement movement;
    private Jump jump;

    private bool _f;
    private bool flipped
    {
        get
        {
            return _f;
        }
        set
        {
            _f = value;
            spriteRenderer.flipX = value;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();

        movementManager = GetComponent<MovementManager>();
        movement = GetComponent<Movement>();
        jump = GetComponent<Jump>();
    }

    private void OnGrounded()
    {
        animator.SetBool("IsGrounded", true);
    }

    private void OnUngrounded()
    {
        animator.SetBool("IsGrounded", false);
    }

    private void OnJumped()
    {
        animator.Play("Jump");
    }

    private void OnStoppedJump()
    {
        animator.Play("StartFalling");
    }

    private void OnMoveDirChanged(float moveDir)
    {
        animator.SetFloat("x", moveDir == 0 ? 0f : 1f);
        if (moveDir == 0f) return;
        flipped = moveDir < 0f;
    }

    private void OnEnable()
    {
        movementManager.onGrounded += OnGrounded;
        movementManager.onUngrounded += OnUngrounded;
        jump.onJumped += OnJumped;
        jump.onJumpEnded += OnStoppedJump;
        movement.onMoveDirChanged += OnMoveDirChanged;
    }

    private void OnDisable()
    {
        movementManager.onGrounded -= OnGrounded;
        movementManager.onUngrounded -= OnUngrounded;
        jump.onJumped -= OnJumped;
        jump.onJumpEnded -= OnStoppedJump;
        movement.onMoveDirChanged -= OnMoveDirChanged;
    }
}
