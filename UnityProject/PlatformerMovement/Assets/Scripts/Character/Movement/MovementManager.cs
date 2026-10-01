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
            rigidbody.linearVelocityX = NormalizedFloat.NormalizeFloat(rigidbody.linearVelocityX);
        }
    }

    private void InAirSpeedControll()
    {
        if (Mathf.Abs(rigidbody.linearVelocityY) > maxFallingSpeed)
        {
            rigidbody.linearVelocityY = NormalizedFloat.NormalizeFloat(rigidbody.linearVelocityY) * maxFallingSpeed;
        }
    }

    private void OnJump()
    {
        linearDamping = airFriction;
        canChangeLinearDamping = false;
        movementState = MovementState.Jumping;
    }

    private void OnJumpEnded()
    {
        if (movementState == MovementState.Dashing) return;
        movementState = MovementState.Default;
        canChangeLinearDamping = true;
        SetLinearDamping();
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
        movementState = MovementState.Default;
        canChangeLinearDamping = true;
        canChangeUseGravity = true;
        SetLinearDamping();
        SetUseGravity();
    }

    private void OnEnable()
    {
        jump.onJumped += OnJump;
        jump.onJumpEnded += OnJumpEnded;
        run.onStartedRunning += OnStartedRunning;
        run.onStoppedRunning += OnStoppedRunning;
        dash.onDashStart += OnDashStart;
        dash.onDashEnd += OnDashEnd;
    }

    private void OnDisable()
    {
        jump.onJumped -= OnJump;
        jump.onJumpEnded -= OnJumpEnded;
        run.onStartedRunning -= OnStartedRunning;
        run.onStoppedRunning -= OnStoppedRunning;
        dash.onDashStart -= OnDashStart;
        dash.onDashEnd -= OnDashEnd;
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
        Dashing
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            useGravity = useGravity;
    }
#endif
}
