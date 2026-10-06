using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementManager))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D rigidbody;

    private MovementManager movementManager;

    [SerializeField] private float _normalSpeed = 3f;
    public float normalSpeed { get { return _normalSpeed; } }

    private Vector2 _lHD;
    private Vector2 lastHorizontalDir
    {
        get { return _lHD; }
        set
        {
            _lHD = value;
            onMoveDirChanged?.Invoke(value);
        }
    }

    public event Action<Vector2> onMoveDirChanged;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        movementManager = GetComponent<MovementManager>();
    }

    private void FixedUpdate()
    {
        if (movementManager.movementState == MovementManager.MovementState.Dashing) return;
        Move();
    }

    protected virtual void Move()
    {

    }

    protected void Move(Vector2 moveDir)
    {
        if (lastHorizontalDir != moveDir)
            lastHorizontalDir = moveDir;
        rigidbody.AddForce(moveDir.normalized * movementManager.curSpeed * 10f, ForceMode2D.Force);
    }
}
