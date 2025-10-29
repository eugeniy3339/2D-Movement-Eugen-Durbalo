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

    [SerializeField] protected float _jumpHeight;
    [SerializeField] protected float _minJumpCooldowm = 0.1f;
    protected float _jumpCooldown;
    protected float _curJumpCooldown;

    protected bool _canJump = true;
    protected bool _jumping;
    protected bool _canceledJump;

    protected virtual void Start()
    {
        _player = Player.Instance;
        _rigidbody = GetComponentInChildren<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

        _jumpCooldown = _minJumpCooldowm;
    }

    private void Update()
    {
        ResetJump();
    }

    public virtual void OnJump(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            JumpAction();
        }
        else if(context.canceled)
        {
            CancelJumpAction();
        }
    }

    protected virtual void JumpAction()
    {
        if(!_canJump) return;

        if (_player.movementScript.isGrounded)
        {
            _jumping = true;
            _player.movementScript.slopesSpeedControl = false;
            _player.movementScript.isGrounded = false;
            _animator.SetBool("InAir", true);
            _canJump = false;
            _canceledJump = false;
            _curJumpCooldown = 0f;
            _player.movementScript.checkIfIsGrounded = false;
            _jumpCooldown = _minJumpCooldowm;

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
            _rigidbody.AddForce(transform.up * _jumpHeight, ForceMode2D.Impulse);

            _animator.Play("Jumping");
        }
    }

    protected virtual void CancelJumpAction()
    {
        if (!_canceledJump && _jumping && _rigidbody.linearVelocity.y > 0f)
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
            _player.movementScript.SetGravityScale(false);
            _canceledJump = true;
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
                _player.movementScript.canWalk = true;
                _player.movementScript.checkIfIsGrounded = true;
                if (_player.movementScript.isGrounded)
                {
                    _jumping = false;
                    _player.movementScript.slopesSpeedControl = true;
                    _canJump = true;
                }
            }
        }
    }
}