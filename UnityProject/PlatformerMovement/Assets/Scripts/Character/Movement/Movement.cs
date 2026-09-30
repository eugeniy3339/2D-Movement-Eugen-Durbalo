using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private MovementManager movementManager;

    [SerializeField] private float _normalSpeed = 3f;
    public float normalSpeed { get { return _normalSpeed; } }
    [SerializeField] private float airMultiplier = 0.8f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        movementManager = GetComponent<MovementManager>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected virtual void Move()
    {
        
    }

    protected void Move(float dir)
    {
        float airMultiplier = movementManager.isGrounded ? 1f : this.airMultiplier;
        if (airMultiplier == 0) return;

        Vector2 moveDirection = new Vector2(dir, 0f);
        if (movementManager.onSlope) 
            moveDirection = GetSlopeMoveDir(moveDirection, movementManager.groundHit);

        rigidbody.AddForce(moveDirection.normalized * movementManager.curSpeed * airMultiplier * 10f, ForceMode2D.Force);
    }

    private Vector2 GetSlopeMoveDir(Vector2 moveDir, RaycastHit2D raycastHit)
    {
        return Vector3.ProjectOnPlane(moveDir, raycastHit.normal);
    }
}
