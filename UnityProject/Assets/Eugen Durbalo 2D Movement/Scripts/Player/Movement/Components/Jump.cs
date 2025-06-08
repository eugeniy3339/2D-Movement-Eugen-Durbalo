using UnityEngine;

/*
 
This script is a module for player movement and wouldnt work without a player, player movement and player inputs manager scripts. (doesnt work with TopDown movement)
 
 */

public class Jump : MonoBehaviour
{
    private Player _playerScript;
    private Rigidbody2D _rb;
    private Animator _animator;

    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _jumpCooldown = 0.1f;

    private bool _canJump = true;

    private void Awake()
    {
        _playerScript = GetComponentInChildren<Player>();
        _rb = GetComponentInChildren<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    public void JumpAction()
    {
        if(!_canJump) return;

        if(_playerScript.playerMovementType == PlayerMovementType.Platformer)
        {
            if(GetComponent<PlatformerMovement>().isGrounded)
            {
                GetComponent<PlatformerMovement>().slopesSpeedControl = false;
                _canJump = false;

                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
                _rb.AddForce(transform.up * _jumpHeight, ForceMode2D.Impulse);

                _animator.Play("Jumping");

                Invoke("ResetJump", _jumpCooldown);
            }
        }
    }

    private void ResetJump()
    {
        GetComponent<PlatformerMovement>().slopesSpeedControl = true;
        _canJump = true;
    }
}
