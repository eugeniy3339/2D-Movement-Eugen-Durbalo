using System;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private Rigidbody2D rigidbody;

    private Movement movement;
    private Dash dash;
    private Run run;

    public float curSpeed { get; private set; }

    private MovementState _mS;
    public MovementState movementState { get { return _mS; } private set {
            if (value == _mS) return;
            _mS = value; 
            onMovementStateChanged?.Invoke(value); 
        } 
    }

    [SerializeField] private float defaultLinearDamping = 15f;
    private bool canChangeLinearDamping = true;
    private float _lD;
    private float linearDamping
    {
        get { return _lD; }
        set
        {
            if (!canChangeLinearDamping) return;
            _lD = value;
            rigidbody.linearDamping = value;
        }
    }

    public event Action<MovementState> onMovementStateChanged;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        movement = GetComponent<Movement>();
        dash = GetComponent<Dash>();
        run = GetComponent<Run>();

        curSpeed = movement != null ? movement.normalSpeed : 0f;

        canChangeLinearDamping = true;
        linearDamping = defaultLinearDamping;
    }

    private void Update()
    {
        SpeedControll();
    }

    private void SpeedControll()
    {
        if (movementState == MovementState.Dashing) return;

        if(rigidbody.linearVelocity.magnitude > curSpeed)
        {
            rigidbody.linearVelocity = rigidbody.linearVelocity.normalized * curSpeed;
        }
    }

    private void OnDashStarted(Vector2 direction)
    {
        movementState = MovementState.Dashing;
        linearDamping = 0f;
        canChangeLinearDamping = false;
    }

    private void OnDashEnded()
    {
        movementState = MovementState.Default;
        canChangeLinearDamping = true;
        linearDamping = defaultLinearDamping;
    }

    private void OnStartedRunning()
    {
        curSpeed = run.runSpeed;
    }

    private void OnStoppedRunning()
    {
        curSpeed = movement != null ? movement.normalSpeed : 0f;
    }



    private void OnEnable()
    {
        if(dash != null)
        {
            dash.onDashStarted += OnDashStarted;
            dash.onDashEnded += OnDashEnded;
        }
        if (run != null)
        {
            run.onStartedRunning += OnStartedRunning;
            run.onStoppedRunning += OnStoppedRunning;
        }
    }

    private void OnDisable()
    {
        if(dash != null)
        {
            dash.onDashStarted -= OnDashStarted;
            dash.onDashEnded -= OnDashEnded;
        }
        if(run != null)
        {
            run.onStartedRunning -= OnStartedRunning;
            run.onStoppedRunning -= OnStoppedRunning;
        }
    }



    public enum MovementState
    {
        Default,
        Dashing
    }
}
