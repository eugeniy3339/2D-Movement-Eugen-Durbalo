using UnityEngine;

public enum WallJumpCheckType
{
    OnCollisionEnter,
    Raycast
}

public class WallJump : Jump
{
    [Header("Wall Jump")]
    [SerializeField] private WallJumpCheckType _jumpCheckType;
    private float _beforeWallJumpAngle = 70f;
    private Vector2 _wallJumpDirection;

    [SerializeField] private float _wallJumpForce = 30f;
    private float _beforeWallJumpForce = 30f;
    [Tooltip("Direction for jumping to the right side."), SerializeField] private float _wallJumpAngle = 70f;
    [SerializeField] private float _wallJumpCooldown = 0.3f;
    [SerializeField] private float _onWallDrag = 7f;

    private Collider2D _curWall;
    private bool _isWallOnTheLeftSide;

    private bool _onWall;
    public bool onWall { get { return _onWall; } }

    private SpriteRenderer _gfxSpriteRenderer;


    private bool _changingWallJumpAngleOrForce = false;
    //private LineRenderer _lineRenderer;

    private Collision2D _lastWallCollision;

    protected override void Start()
    {
        base.Start();
        _gfxSpriteRenderer = _player.gfx.GetComponent<SpriteRenderer>();

        _wallJumpDirection = GetWallJumpDirection(_wallJumpAngle);
        _beforeWallJumpAngle = _wallJumpAngle;
        _wallJumpDirection = GetWallJumpDirection(_wallJumpAngle);

        _player.movementScript.onGetOnLader += OnGetOnStairs;
    }

    protected override void Update()
    {
        base.Update();
        if(_beforeWallJumpAngle != _wallJumpAngle)
        {
            _beforeWallJumpAngle = _wallJumpAngle;
            _wallJumpDirection = GetWallJumpDirection(_wallJumpAngle);
        }

        if(_onWall && _player.movementScript.isGrounded)
        {
            ExitWall();
        }
    }

    protected override void JumpAction(Vector2 direction, float jumpStrengh, bool checkIfIsGrounded)
    {
        if (!_onWall)
            base.JumpAction(Vector2.up, _jumpHeight, true);
        else
        {
            _player.movementScript.canChangeRigidbodyDamping = true;
            print(new Vector2(_wallJumpDirection.x * (_isWallOnTheLeftSide ? 1f : -1f), _wallJumpDirection.y));
            base.JumpAction(new Vector2(_wallJumpDirection.x * (_isWallOnTheLeftSide ? 1f : -1f), _wallJumpDirection.y), _wallJumpForce, false);
            _player.movementScript.canWalk = false;
            _jumpCooldown = _wallJumpCooldown;
            _player.canFlipGfx = true;
            _gfxSpriteRenderer.flipX = !_isWallOnTheLeftSide;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_jumpCheckType != WallJumpCheckType.OnCollisionEnter) return;

        if (!_curWall)
        {
            if(collision.gameObject.tag == "Wall")
            {
                _lastWallCollision = collision;
                if (!_player.movementScript.isGrounded)
                {
                    GetOnWall(collision);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_jumpCheckType != WallJumpCheckType.OnCollisionEnter) return;

        if (_lastWallCollision != null && collision.collider == _lastWallCollision.collider)
            _lastWallCollision = null;

        if (_curWall)
        {
            if (collision.collider == _curWall)
            {
                ExitWall();
            }
        }
    }

    private void GetOnWall(Collision2D collision)
    {
        if (_player.movementScript.onLader) return;
        print("GetOnWall");
        _curWall = collision.collider;
        _isWallOnTheLeftSide = (collision.GetContact(0).point - new Vector2(transform.position.x, transform.position.y)).x < 0 ? true : false;
        _player.canFlipGfx = false;
        _gfxSpriteRenderer.flipX = !_isWallOnTheLeftSide;
        _player.movementScript.curDrag = _onWallDrag;
        _player.movementScript.canChangeRigidbodyDamping = false;

        _animator.SetBool("OnWall", true);
        _animator.Play("WallSlide");

        _onWall = true;
    }

    private void ExitWall()
    {
        if (!_curWall) return;

        _curWall = null;
        _onWall = false;
        _player.movementScript.canChangeRigidbodyDamping = true;
        _player.movementScript.curDrag = _player.movementScript.isGrounded ? _player.movementScript.groundDrag : 0f;
        _player.canFlipGfx = true;

        _animator.SetBool("OnWall", false);
    }

    private Vector2 GetWallJumpDirection(float angle)
    {
        return new Vector2(Mathf.Cos(_wallJumpAngle * Mathf.Deg2Rad), Mathf.Sin(_wallJumpAngle * Mathf.Deg2Rad)).normalized;
    }

    protected override void CancelJumpAction()
    {
        base.CancelJumpAction();
        if(!_onWall && _lastWallCollision != null)
        {
            GetOnWall(_lastWallCollision);
        }
    }

    private void OnGetOnStairs()
    {
        ExitWall();
    }

    private void OnValidate()
    {
        if(_beforeWallJumpAngle != _wallJumpAngle)
        {
            _wallJumpDirection = GetWallJumpDirection(_wallJumpAngle);
            _beforeWallJumpAngle = _wallJumpAngle;
            _changingWallJumpAngleOrForce = true;
            //DrawJumpLine(_wallJumpDirection, _wallJumpForce, _afterJumpGravityScale, GetComponent<Rigidbody2D>().mass);
        }
        else if(_wallJumpForce != _beforeWallJumpForce)
        {
            _beforeWallJumpForce = _wallJumpForce;
            _changingWallJumpAngleOrForce = true;
            //DrawJumpLine(_wallJumpDirection, _wallJumpForce, _afterJumpGravityScale, GetComponent<Rigidbody2D>().mass);
        }
        else
        {
            _changingWallJumpAngleOrForce = false;
        }
    }

    private void OnDrawGizmos()
    {
        if(_changingWallJumpAngleOrForce)
        {
            Gizmos.DrawLine(transform.position, transform.position + (new Vector3(_wallJumpDirection.x, _wallJumpDirection.y, 0f) * _wallJumpForce/_afterJumpGravityScale));
        }
    }

    /*private void DrawJumpLine(Vector2 dir, float impulse, float gravityScale, float mass)
    {
        if (!_lineRenderer)
        {
            if(transform.GetComponentInChildren<LineRenderer>())
            {
                _lineRenderer = transform.GetComponentInChildren<LineRenderer>();
            }
            else
            {
                GameObject lineRendererObject = new GameObject("PlayerLineRenderer");
                _lineRenderer = lineRendererObject.AddComponent<LineRenderer>();
                _lineRenderer.transform.SetParent(transform, false);
                _lineRenderer.transform.localPosition = Vector3.zero;
            }
        }

        _lineRenderer.positionCount = 0;

        Vector2 mimpulse = impulse * dir/* - Vector2.one * _onWallDrag*//*;
        Vector2 v0 = mimpulse / mass;
        Vector2 a = Physics2D.gravity * gravityScale; // Physics2D.gravity is typically (0,-9.81)
        Vector2 posAtTime(float t) => new Vector2(transform.position.x, transform.position.y) + v0 * t + 0.5f * a * t * t;

        float time = 1f;
        float curTime = 0f;

        while (curTime < time)
        {
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, new Vector3(posAtTime(curTime).x, posAtTime(curTime).y, 0f));
            curTime += 0.01f;
        }
    }*/
}