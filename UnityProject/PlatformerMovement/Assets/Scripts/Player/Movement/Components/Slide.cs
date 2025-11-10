using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.EnhancedTouch;

public enum SlideActivationMode
{
    Run,
    Speed,
    WithoutAnyConditions
}

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Rigidbody2D))]
public class Slide : Crouch
{
    private Rigidbody2D _rigidbody;

    [SerializeField] private SlideActivationMode _slideActivationMode;
    [SerializeField] private float _minSpeedToActivateSlide = 3f;
    [SerializeField] private float _slideStartImpulse = 5f;
    [SerializeField] private float _minSlideSpeed = 0.5f;
    [SerializeField] private float _slideCooldown = 0.2f;
    [SerializeField] private float _slideGroundDrag = 0.5f;
    private float _curSlideCooldown;

    private bool _sliding;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponentInChildren<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
        _player.movementScript.onPlayerStateChanged += OnPlayerStateChanged;
    }

    private void Update()
    {
        if(_curSlideCooldown < _slideCooldown)
        {
            _curSlideCooldown += Time.deltaTime;
        }

        if(_sliding)
        {
            if (_player.movementScript.isGrounded)
            { _player.gfx.right = _player.movementScript.GetSlopeMoveDirection(Vector2.right); }
            else
            { _player.gfx.right = Vector2.right; SlideAction(false); }

            if (_rigidbody.linearVelocity.magnitude <= _minSlideSpeed)
            {
                SlideAction(false);
            }
        }
    }

    public override void CrouchAction(bool crouch)
    {
        bool canStartSlide = false;

        switch(_slideActivationMode)
        {
            case SlideActivationMode.Run:
                if (_player.movementScript.run) canStartSlide = true;
                break;
            case SlideActivationMode.Speed:
                if (_rigidbody.linearVelocity.magnitude >= _minSpeedToActivateSlide) canStartSlide = true;
                break;
            case SlideActivationMode.WithoutAnyConditions:
                canStartSlide = true;
                break;
        }

        if(canStartSlide || _sliding)
        {
            SlideAction(crouch);
        }
        else
        {
            base.CrouchAction(crouch);
        }
    }

    private void SlideAction(bool slide)
    {
        if (_player.movementScript.onLader || (_player.GetComponent<WallJump>() && _player.GetComponent<WallJump>().onWall)) return;
        if (_curSlideCooldown < _slideCooldown && slide) return; 

        if (slide && _player.movementScript.playerState == PlayerState.Movement && _player.movementScript.isGrounded && !_sliding)
        {
            _curSlideCooldown = 0f;
            Vector2 moveDir = new Vector2(_player.movementScript.lastMoveInputX, 0f);
            if(_player.movementScript.OnSlope())
            {
                moveDir = _player.movementScript.GetSlopeMoveDirection(moveDir);
            }
            _player.movementScript.curDrag = _slideGroundDrag;
            _player.movementScript.curGravityScale = _player.movementScript.normalGravityScale;
            if(_player.movementScript.isGrounded) _rigidbody.AddForce(moveDir * _slideStartImpulse, ForceMode2D.Impulse);
            _animator.SetFloat("x", 1f);

            _player.movementScript.canChangeRigidbodyDamping = false;
            _player.movementScript.canChangeGravityScale = false;
            _player.movementScript.canWalk = false;
            _sliding = true;
            _animator.SetBool("Crouching", true);
        }
        else
        {
            if (!_sliding) return;
            _player.movementScript.canChangeRigidbodyDamping = true;
            _player.movementScript.canChangeGravityScale = true;
            _player.movementScript.curDrag = _player.movementScript.isGrounded ? _player.movementScript.groundDrag : 0f;
            _player.gfx.up = Vector2.up;

            _player.movementScript.canWalk = true;
            _sliding = false;
            _animator.SetBool("Crouching", false);
        }
    }

    private void OnPlayerStateChanged(PlayerState beforeState, PlayerState curState)
    {
        if(_sliding && curState != PlayerState.Movement)
        {
            SlideAction(false);
        }
    }
}
