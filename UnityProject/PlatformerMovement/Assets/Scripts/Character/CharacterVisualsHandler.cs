using UnityEngine;

public class CharacterVisualsHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private MovementManager movementManager;
    private Movement movement;
    private Jump jump;
    private Run run;
    private Dash dash;

    private float moveDir;

    private bool _f;
    private bool flipped
    {
        get
        {
            return _f;
        }
        set
        {
            if (dashing) return;
            _f = value;
            spriteRenderer.flipX = value;
        }
    }

    private bool running;
    private bool _d;
    private bool dashing
    {
        get
        {
            return _d;
        }
        set
        {
            _d = value;
            animator.SetBool("IsDashing", value);
            if(!value)
                SetMoveDir(moveDir);
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();

        movementManager = GetComponent<MovementManager>();
        movement = GetComponent<Movement>();
        jump = GetComponent<Jump>();
        run = GetComponent<Run>();
        dash = GetComponent<Dash>();
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

    private void OnStartedRunning()
    {
        SetRun(true);
    }

    private void OnStoppedRunning()
    {
        SetRun(false);
    }

    private void SetRun(bool run)
    {
        running = run;
        SetMoveDir(moveDir);
    }

    private void OnMoveDirChanged(float moveDir)
    {
        SetMoveDir(moveDir);
    }

    private void SetMoveDir(float moveDir)
    {
        this.moveDir = moveDir;
        animator.SetFloat("x", moveDir == 0 ? 0f : (running ? 1f : 0.5f));
        if (moveDir == 0f) return;
        flipped = moveDir < 0f;
    }

    private void OnDashStart()
    {
        dashing = true;
        animator.Play("Dash");
    }

    private void OnDashEnd()
    {
        dashing = false;
    }

    private void OnEnable()
    {
        movementManager.onGrounded += OnGrounded;
        movementManager.onUngrounded += OnUngrounded;
        jump.onJumped += OnJumped;
        jump.onJumpEnded += OnStoppedJump;
        movement.onMoveDirChanged += OnMoveDirChanged;
        run.onStartedRunning += OnStartedRunning;
        run.onStoppedRunning += OnStoppedRunning;
        dash.onDashStart += OnDashStart;
        dash.onDashEnd += OnDashEnd;
    }

    private void OnDisable()
    {
        movementManager.onGrounded -= OnGrounded;
        movementManager.onUngrounded -= OnUngrounded;
        jump.onJumped -= OnJumped;
        jump.onJumpEnded -= OnStoppedJump;
        movement.onMoveDirChanged -= OnMoveDirChanged;
        run.onStartedRunning -= OnStartedRunning;
        run.onStoppedRunning -= OnStoppedRunning;
        dash.onDashStart -= OnDashStart;
        dash.onDashEnd -= OnDashEnd;
    }
}
