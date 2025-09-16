using UnityEngine;
using UnityEngine.InputSystem;

/*
 
This script is a module for player movement and wouldnt work without a player, player movement and player inputs manager scripts. (doesnt work with TopDown movement)
 
 */

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Rigidbody))]
public class Jump : PlayerComponent
{
    private Player _player;
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _minJumpCooldowm = 0.1f;
    private float _curJumpCooldown;

    private bool _canJump = true;
    private bool _jumping;
    private bool _canceledJump;

    private void Start()
    {
        _player = Player.Instance;
        _rigidbody = GetComponentInChildren<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        ResetJump();
    }

    public void OnJump(InputAction.CallbackContext context)
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

    private void JumpAction()
    {
        if(!_canJump) return;

        if (_player.movementScript.isGrounded)
        {
            _jumping = true;
            _player.movementScript.slopesSpeedControl = false;
            _canJump = false;
            _canceledJump = false;
            _curJumpCooldown = 0f;

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
            _rigidbody.AddForce(transform.up * _jumpHeight, ForceMode2D.Impulse);

            _animator.Play("Jumping");
        }
    }

    private void CancelJumpAction()
    {
        if (!_canceledJump && _jumping && _rigidbody.linearVelocity.y > 0f)
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
            _player.movementScript.SetGravityScale(false);
            _canceledJump = true;
        }
    }

    private void ResetJump()
    {
        if(_curJumpCooldown < _minJumpCooldowm)
        {
            _curJumpCooldown += Time.deltaTime;
        }
        else
        {
            if (_jumping)
            {
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
