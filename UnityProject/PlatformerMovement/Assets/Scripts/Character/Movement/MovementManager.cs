using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementManager : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private Character character;

    private Movement movement;
    private Jump jump;
    private Run run;
    private Dash dash;

    private MovementState _mS;
    public MovementState movementState
    {
        get { return _mS; }
        set
        {
            _mS = value;
            onMovementStateChanged?.Invoke(value);
        }
    }

    public float curSpeed { get; private set; }
    [SerializeField] private float maxFallingSpeed = 50f;

    [SerializeField] private float groundFriction = 10f;
    [SerializeField] private float airFriction = 0f;

    [SerializeField] private Transform feetPos;
    [SerializeField] private float sphereCastRadius = 0.3f;
    [SerializeField] private float sphereCastDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxSlopeAngle = 40f;
    public bool isGrounded { get; private set; }
    private RaycastHit2D _groundHit;
    public RaycastHit2D groundHit { get { return _groundHit; } }
    public bool onSlope { get; private set; }

    [SerializeField] private bool canClimbTheWalls = true;
    private const string WALL_TAG = "Wall";
    private List<Collider2D> collidingWalls = new List<Collider2D>();
    public Collider2D curWall { get; private set; }
    [SerializeField] private float wallFriction = 10f;

    [SerializeField] private float defaultGravityScale = 2f;
    private bool canChangeUseGravity = true;
    private bool _uG;
    public bool useGravity
    {
        get
        {
            return _uG;
        }
        set
        {
            if (!canChangeUseGravity) return;
            if (rigidbody)
                rigidbody.gravityScale = value ? defaultGravityScale : 0f;
            _uG = value;
        }
    }

    private bool canChangeLinearDamping = true;
    private float _lD;
    public float linearDamping
    {
        get
        {
            return _lD;
        }
        set
        {
            if (!canChangeLinearDamping) return;
            if (rigidbody)
                rigidbody.linearDamping = value;
            _lD = value;
        }
    }

    public event Action<MovementState> onMovementStateChanged;
    public event Action onGrounded;
    public event Action onUngrounded;
    public event Action onGotOnSlope;
    public event Action onGotOfSlope;
    public event Action<Collider2D> onGotOnTheWall;
    public event Action OnGotOfWall;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        character = GetComponent<Character>();

        movement = GetComponent<Movement>();
        jump = GetComponent<Jump>();
        run = GetComponent<Run>();
        dash = GetComponent<Dash>();

        if (feetPos == null)
        {
            feetPos = CreateFeetPos();
        }

        curSpeed = movement.normalSpeed;
        useGravity = true;

        movementState = MovementState.Default;
    }

    private void Start()
    {

    }

    private void Update()
    {
        bool isGrounded = IsGrounded(out _groundHit);
        if (this.isGrounded != isGrounded)
        {
            this.isGrounded = isGrounded;
            SetLinearDamping();

            if (isGrounded)
            {
                onGrounded?.Invoke();
            }
            else
            {
                onUngrounded?.Invoke();
            }
        }

        bool onSlope = OnSlope(_groundHit);
        if (this.onSlope != onSlope)
        {
            this.onSlope = onSlope;
            SetUseGravity();

            if (onSlope)
            {
                onGotOnSlope?.Invoke();
            }
            else
            {
                onGotOfSlope?.Invoke();
            }
        }

        SpeedControll();
    }

    private void SetLinearDamping()
    {
        linearDamping = isGrounded ? groundFriction : airFriction;
    }

    private void SetUseGravity()
    {
        useGravity = !onSlope;
    }

    private bool IsGrounded(out RaycastHit2D hit)
    {
        Vector2 startPos = new Vector2(feetPos.transform.position.x, feetPos.transform.position.y + sphereCastRadius);
        hit = Physics2D.CircleCast(startPos, sphereCastRadius, Vector2.down, sphereCastDistance, groundLayer);
        return hit;
    }

    private bool OnSlope(RaycastHit2D hit)
    {
        float angle = Vector2.Angle(Vector2.up, hit.normal);
        return angle > 0 && angle < maxSlopeAngle;
    }

    private void SpeedControll()
    {
        if(movementState == MovementState.Dashing) return;

        if (onSlope)
            SlopeSpeedControll();
        else
            FlatSpeedControll();

        if (!isGrounded)
            InAirSpeedControll();
    }

    private void SlopeSpeedControll()
    {
        if (rigidbody.linearVelocity.magnitude > curSpeed)
            rigidbody.linearVelocity = rigidbody.linearVelocity.normalized * curSpeed;
    }

    private void FlatSpeedControll()
    {
        if (Mathf.Abs(rigidbody.linearVelocityX) > curSpeed)
        {
            rigidbody.linearVelocityX = NormalizedFloat.NormalizeFloat(rigidbody.linearVelocityX) * curSpeed;
        }
    }

    private void InAirSpeedControll()
    {
        if (Mathf.Abs(rigidbody.linearVelocityY) > maxFallingSpeed)
        {
            rigidbody.linearVelocityY = NormalizedFloat.NormalizeFloat(rigidbody.linearVelocityY) * maxFallingSpeed;
        }
    }



    //Wall Climbing


    private void OnCollisionEnter2D(Collision2D collision)
    {
        AddWall(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        RemoveWall(collision);
    }



    private void AddWall(Collision2D collision)
    {
        if (collision.collider.tag != WALL_TAG) return;
        if (collidingWalls.Contains(collision.collider)) return;

        /*Vector2 dirToTheCollisionContact = collision.contacts[0].point - new Vector2(transform.position.x, transform.position.y);
        if (Mathf.Abs(dirToTheCollisionContact.x) > 0.1f)*/
        {
            collidingWalls.Add(collision.collider);
            GetOnTheWallIfCanTo(collision.collider);
        }
    }

    private void GetOnTheWallIfCanTo()
    {
        if (collidingWalls.Count > 0)
            GetOnTheWallIfCanTo(collidingWalls[0]);
    }

    private void GetOnTheWallIfCanTo(Collider2D collider)
    {
        if(CanGetOnTheWall())
            GetOnTheWall(collider);
    }

    private bool CanGetOnTheWall()
    {
        return !isGrounded && curWall == null && movementState == MovementState.Default;
    }

    private void GetOnTheWall(Collider2D collider)
    {
        curWall = collider;
        linearDamping = wallFriction;
        movementState = MovementState.OnWall;
        onGotOnTheWall?.Invoke(collider);
    }

    private void RemoveWall(Collision2D collision)
    {
        if (collidingWalls.Contains(collision.collider))
        {
            collidingWalls.Remove(collision.collider);
            GetOfTheWall();
        }
    }

    private void GetOfTheWall()
    {
        if (curWall == null) return;

        curWall = null;
        SetLinearDamping();
        if(movementState == MovementState.OnWall)
            movementState = MovementState.Default;
        OnGotOfWall?.Invoke();
    }


    //Wall Climbing



    private void OnMovementStateChanged(MovementState movementState)
    {
        if (movementState == MovementState.Default)
        {
            GetOnTheWallIfCanTo();
        }
        else if(movementState != MovementState.OnWall)
        {
            GetOfTheWall();
        }
    }

    private void OnUngrounded()
    {
        GetOnTheWallIfCanTo();
    }

    private void OnGrounded()
    {
        GetOfTheWall();
    }

    private void OnJump()
    {
        linearDamping = airFriction;
        canChangeLinearDamping = false;
        movementState = MovementState.Jumping;
    }

    private void OnWallJump(Vector2 jumpDir)
    {
        linearDamping = airFriction;
        canChangeLinearDamping = false;
        movementState = MovementState.JumpingOfWall;
    }

    private void OnJumpEnded()
    {
        if (movementState == MovementState.Dashing) return;
        canChangeLinearDamping = true;
        SetLinearDamping();
        movementState = MovementState.Default;
    }

    private void OnStartedRunning()
    {
        curSpeed = run.runningSpeed;
    }

    private void OnStoppedRunning()
    {
        curSpeed = movement.normalSpeed;
    }

    private void OnDashStart()
    {
        movementState = MovementState.Dashing;
        canChangeLinearDamping = true;
        canChangeUseGravity = true;
        linearDamping = airFriction;
        useGravity = false;
        canChangeLinearDamping = false;
        canChangeUseGravity = false;
    }

    private void OnDashEnd()
    {
        canChangeLinearDamping = true;
        canChangeUseGravity = true;
        SetLinearDamping();
        SetUseGravity();
        movementState = MovementState.Default;
    }

    private void OnEnable()
    {
        jump.onJumped += OnJump;
        jump.onWallJumped += OnWallJump;
        jump.onJumpStopped += OnJumpEnded;
        run.onStartedRunning += OnStartedRunning;
        run.onStoppedRunning += OnStoppedRunning;
        dash.onDashStart += OnDashStart;
        dash.onDashEnd += OnDashEnd;
        onMovementStateChanged += OnMovementStateChanged;
        onUngrounded += OnUngrounded;
        onGrounded += OnGrounded;
    }

    private void OnDisable()
    {
        jump.onJumped -= OnJump;
        jump.onWallJumped -= OnWallJump;
        jump.onJumpStopped -= OnJumpEnded;
        run.onStartedRunning -= OnStartedRunning;
        run.onStoppedRunning -= OnStoppedRunning;
        dash.onDashStart -= OnDashStart;
        dash.onDashEnd -= OnDashEnd;
        onMovementStateChanged -= OnMovementStateChanged;
        onUngrounded -= OnUngrounded;
        onGrounded -= OnGrounded;
    }



    private Transform CreateFeetPos()
    {
        GameObject feetPos = new GameObject("FeetPos");
        feetPos.transform.parent = transform;
        feetPos.transform.localPosition = Vector3.zero;
        return feetPos.transform;
    }



    public enum MovementState
    {
        Default,
        Jumping,
        Dashing,
        OnWall,
        JumpingOfWall
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            useGravity = useGravity;
    }
#endif
}
