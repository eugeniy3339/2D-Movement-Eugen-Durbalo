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
    private float _curSlideCooldown;

    private bool _sliding;

    protected override void Start()
    {
        base.Start();
        _rigidbody = GetComponentInChildren<Rigidbody2D>();
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
            { _player.gfx.right = Vector2.right; }

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
        if (_curSlideCooldown < _slideCooldown && slide) return; 
        //if (!_player.movementScript.canWalk) return;
        _sliding = slide;
        _player.movementScript.canWalk = !slide;
        _player.movementScript.canChangeRigidbodyDamping = !slide;
        _player.movementScript.canChangeUseGravity = !slide;
        _animator.SetBool("Crouching", slide);
        if (slide)
        {
            _curSlideCooldown = 0f;
            Vector2 moveDir = new Vector2(_player.movementScript.lastMoveInputX, 0f);
            if(_player.movementScript.OnSlope())
            {
                moveDir = _player.movementScript.GetSlopeMoveDirection(moveDir);
            }
            _rigidbody.linearDamping = 0f;
            if(_rigidbody.gravityScale == 0f) _rigidbody.gravityScale = 1f;
            if(_player.movementScript.isGrounded) _rigidbody.AddForce(moveDir * _slideStartImpulse, ForceMode2D.Impulse);
            _animator.SetFloat("x", 1f);
        }
        else
        {
            _rigidbody.linearDamping = _player.movementScript.groundDrag;
            _player.gfx.up = Vector2.up;
        }
    }
}
