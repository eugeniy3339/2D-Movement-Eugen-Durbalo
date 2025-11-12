using UnityEngine;
using UnityEngine.InputSystem;

/*
 
This script is a module for player movement and wouldnt work without a player, player movement and player inputs manager scripts. (doesnt work with TopDown movement)
 
 */

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Rigidbody2D))]
public class Jump : PlayerComponent
{
    protected Player _player;
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;

    [Header("Jump")]
    [SerializeField] protected float _jumpHeight = 30f;
    [SerializeField] protected float _minJumpTime = 0.1f;
    protected float _jumpCooldown;
    protected float _curJumpCooldown;

    protected bool _canJump = true;
    protected bool _jumping;
    protected bool _canceledJump;

    [SerializeField] protected float _afterJumpGravityScale = 5f;

    protected virtual void Start()
    {
        _player = Player.Instance;
        _rigidbody = GetComponentInChildren<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

        _jumpCooldown = _minJumpTime;
    }

    protected virtual void Update()
    {
        CancelJumpIfFalling();
        ResetJump();
    }

    protected virtual void CancelJumpIfFalling()
    {
        if(_jumping)
        {
            if (_rigidbody.linearVelocity.y <= 0f && !_canceledJump)
            {
                CancelJumpAction();
            }
        }
    }

    public virtual void OnJump(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            JumpAction(Vector2.up, _jumpHeight, true);
        }
        else if(context.canceled)
        {
            CancelJumpAction();
        }
    }

    protected virtual void JumpAction(Vector2 direction, float jumpStrengh, bool checkIfIsGrounded)
    {
        if(!_canJump) return;

        if ((_player.movementScript.onLadder || !checkIfIsGrounded || _player.movementScript.isGrounded) && _player.movementScript.playerState == PlayerState.Movement)
        {
            _jumping = true;
            _player.movementScript.GetOfLadder();
            _player.movementScript.curDrag = 0f;
            _player.movementScript.slopesSpeedControl = false;
            _player.movementScript.isGrounded = false;
            _animator.SetBool("InAir", true);
            _canJump = false;
            _canceledJump = false;
            _curJumpCooldown = 0f;
            _player.movementScript.checkIfIsGrounded = false;
            _jumpCooldown = _minJumpTime;

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
            _rigidbody.AddForce(direction * jumpStrengh, ForceMode2D.Impulse);

            _animator.Play("Jumping");
        }
    }

    protected virtual void CancelJumpAction()
    {
        if (!_canceledJump && _jumping)
        {
            _player.movementScript.curGravityScale = _afterJumpGravityScale;
            _canceledJump = true;
            _animator.Play(_animator.GetFloat("x") > 0f ? "FallingWhileMoving" : "Falling");
            if (_player.movementScript.getOnTheLadderMehode != GetOnTheLadderMethode.Input)
                _player.movementScript.GetOnLadder();
        }
    }

    protected virtual void ResetJump()
    {
        if(_curJumpCooldown < _jumpCooldown)
        {
            _curJumpCooldown += Time.deltaTime;
        }
        else
        {
            if (_jumping)
            {
                if(_player.movementScript.playerState == PlayerState.Movement) _player.movementScript.canWalk = true;
                _player.movementScript.checkIfIsGrounded = true;
                _canJump = true;
                if (_player.movementScript.isGrounded)
                {
                    _jumping = false;
                    _player.movementScript.slopesSpeedControl = true;
                }
            }
        }
    }
}