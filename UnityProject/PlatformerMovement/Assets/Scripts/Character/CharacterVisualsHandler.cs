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
    private float _lD;
    private float lookDir
    {
        get { return _lD; }
        set
        {
            _lD = value;
            if(value != 0)
            {
                flipped = value < 0f;
            }
        }
    }
    private bool _f;
    private bool flipped
    {
        get { return _f; }
        set
        {
            _f = value;
            spriteRenderer.flipX = value;
        }
    }

    private bool _iG;
    private bool isGrounded
    {
        get { return _iG; }
        set
        {
            _iG = value;
            animator.SetBool("IsGrounded", value);
        }
    }
    private bool _d;
    private bool dashing
    {
        get { return _d; }
        set
        {
            _d = value;
            animator.SetBool("IsDashing", value);
        }
    }
    private bool _oW;
    private bool onWall
    {
        get { return _oW; }
        set
        {
            _oW = value;
            animator.SetBool("IsOnWall", value);
        }
    }
    private bool jumpingFromTheWall;

    private bool running;

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

    private void OnMoveDirChanged(float moveDir)
    {
        SetMoveDir(moveDir);
    }

    private void SetMoveDir(float moveDir)
    {
        this.moveDir = moveDir;
        animator.SetFloat("x", moveDir == 0f ? 0f : (running ? 1f : 0.5f));
        if (!dashing && !onWall && !jumpingFromTheWall)
            SetLookDir(moveDir);
    }

    private void SetLookDir(float lookDir)
    {
        this.lookDir = lookDir;
    }

    private void OnGrounded()
    {
        isGrounded = true;
    }

    private void OnUngrounded()
    {
        isGrounded = false;
    }

    private void OnJumped()
    {
        animator.Play("Jump");
    }

    private void OnWallJumped(Vector2 jumpDir)
    {
        jumpingFromTheWall = true;
        animator.Play("Jump");
        SetLookDir(jumpDir.x);
    }

    private void OnJumpStopped()
    {
        jumpingFromTheWall = false;
        if(!onWall)
            animator.Play("StartFalling");
        SetMoveDir(moveDir);
    }

    private void OnGotOnTheWall(Collider2D wall)
    {
        if (jumpingFromTheWall) return;
        onWall = true;
        animator.Play("OnWall");
        float dirFromTheWall = transform.position.x - wall.transform.position.x;
        SetLookDir(dirFromTheWall);
    }

    private void OnGotOfWall()
    {
        onWall = false;
        SetMoveDir(moveDir);
    }

    private void OnDashStart()
    {
        dashing = true;
        animator.Play("Dash");
        SetLookDir(moveDir);
    }

    private void OnDashEnd()
    {
        dashing = false;
        SetMoveDir(moveDir);
    }

    private void OnStartedRunning()
    {
        running = true;
        SetMoveDir(moveDir);
    }

    private void OnStoppedRunning()
    {
        running = false;
        SetMoveDir(moveDir);
    }

    private void OnEnable()
    {
        movement.onMoveDirChanged += OnMoveDirChanged;
        movementManager.onGrounded += OnGrounded;
        movementManager.onUngrounded += OnUngrounded;
        jump.onJumped += OnJumped;
        jump.onWallJumped += OnWallJumped;
        jump.onJumpStopped += OnJumpStopped;
        movementManager.onGotOnTheWall += OnGotOnTheWall;
        movementManager.OnGotOfWall += OnGotOfWall;
        dash.onDashStart += OnDashStart;
        dash.onDashEnd += OnDashEnd;
        run.onStartedRunning += OnStartedRunning;
        run.onStoppedRunning += OnStoppedRunning;
    }

    private void OnDisable()
    {
        movement.onMoveDirChanged -= OnMoveDirChanged;
        movementManager.onGrounded -= OnGrounded;
        movementManager.onUngrounded -= OnUngrounded;
        jump.onJumped -= OnJumped;
        jump.onWallJumped -= OnWallJumped;
        jump.onJumpStopped -= OnJumpStopped;
        movementManager.onGotOnTheWall -= OnGotOnTheWall;
        movementManager.OnGotOfWall -= OnGotOfWall;
        dash.onDashStart -= OnDashStart;
        dash.onDashEnd -= OnDashEnd;
        run.onStartedRunning -= OnStartedRunning;
        run.onStoppedRunning -= OnStoppedRunning;
    }
}
