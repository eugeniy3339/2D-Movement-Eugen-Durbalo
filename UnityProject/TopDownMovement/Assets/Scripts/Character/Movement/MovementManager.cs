using System;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private Rigidbody2D rigidbody;

    private Movement movement;
    private Dash dash;

    public float curSpeed { get; private set; }

    private MovementState _mS;
    public MovementState movementState { get { return _mS; } private set {
            if (value == _mS) return;
            _mS = value; 
            onMovementStateChanged?.Invoke(value); 
        } 
    }

    public event Action<MovementState> onMovementStateChanged;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        movement = GetComponent<Movement>();
        dash = GetComponent<Dash>();

        curSpeed = movement.normalSpeed;
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
    }

    private void OnDashEnded()
    {
        movementState = MovementState.Default;
    }



    private void OnEnable()
    {
        if(dash != null)
        {
            dash.onDashStarted += OnDashStarted;
            dash.onDashEnded += OnDashEnded;
        }
    }

    private void OnDisable()
    {
        if(dash != null)
        {
            dash.onDashStarted -= OnDashStarted;
            dash.onDashEnded -= OnDashEnded;
        }
    }



    public enum MovementState
    {
        Default,
        Dashing
    }
}
