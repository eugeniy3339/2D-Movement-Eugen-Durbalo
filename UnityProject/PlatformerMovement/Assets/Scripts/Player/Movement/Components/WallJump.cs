using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum WallJumpCheckType
{
    OnCollisionEnter,
    Raycast
}

public class WallJump : Jump
{
    [SerializeField] private WallJumpCheckType _jumpCheckType;
    [Tooltip("Direction for jumping to the right side. Will be normalized."), SerializeField] private Vector2 _wallJumpDirection;

    [SerializeField] private float _wallJumpForce;
    [SerializeField] private float _onWallDrag = 7f;

    [SerializeField] private float _wallJumpCooldown = 0.3f;

    private Collider2D _curWall;
    private bool _isWallOnTheLeftSide;

    private bool _onWall;

    private SpriteRenderer _gfxSpriteRenderer;

    private void Awake()
    {
        _wallJumpDirection = _wallJumpDirection.normalized;
    }

    protected override void Start()
    {
        base.Start();
        _gfxSpriteRenderer = _player.gfx.GetComponent<SpriteRenderer>();
    }

    protected override void JumpAction()
    {
        if (!_onWall)
            base.JumpAction();
        else
        {
            _jumping = true;
            _player.movementScript.slopesSpeedControl = false;
            _player.movementScript.isGrounded = false;
            _animator.SetBool("InAir", true);
            _canJump = false;
            _canceledJump = false;
            _curJumpCooldown = 0f;
            _player.movementScript.checkIfIsGrounded = false;
            _player.movementScript.canWalk = false;
            _jumpCooldown = _wallJumpCooldown;

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
            _rigidbody.AddForce(new Vector2(_wallJumpDirection.x * (_isWallOnTheLeftSide ? 1f : -1f), _wallJumpDirection.y) * _wallJumpForce, ForceMode2D.Impulse);

            _gfxSpriteRenderer.flipX = !_isWallOnTheLeftSide;

            _animator.Play("Jumping");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_jumpCheckType != WallJumpCheckType.OnCollisionEnter) return;

        if (!_curWall)
        {
            if(collision.gameObject.tag == "Wall")
            {
                _curWall = collision.collider;
                _isWallOnTheLeftSide = (collision.GetContact(0).point - new Vector2(transform.position.x, transform.position.y)).x < 0 ? true : false;

                _rigidbody.linearDamping = _onWallDrag;

                _player.movementScript.canChangeRigidbodyDamping = false;

                _onWall = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_jumpCheckType != WallJumpCheckType.OnCollisionEnter) return;

        if (_curWall)
        {
            if (collision.collider == _curWall)
            {
                _curWall = null;
                _onWall = false;
                _rigidbody.linearDamping = _player.movementScript.isGrounded ? _player.movementScript.groundDrag : 0f;

                _player.movementScript.canChangeRigidbodyDamping = true;
            }
        }
    }
}