using UnityEngine;
using UnityEngine.InputSystem;

public enum DashMode
{
    Toggle,
    Hold
}

[RequireComponent(typeof(Movement))]
public class Crouch : PlayerComponent
{
    protected Player _player;
    protected Animator _animator;

    [SerializeField] private float _crouchWalkSpeed = 1f;
    [SerializeField] private DashMode _dashMode;

    private bool _crouch;

    protected virtual void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        _player = Player.Instance;
        _player.movementScript.onGetOnLader += OnGetOnStairs;
    }

    private void Update()
    {
        CrouchSpeedControll();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        switch(_dashMode)
        {
            case DashMode.Hold:
                CrouchAction(!context.canceled);
                break;
            case DashMode.Toggle:
                if(context.started)
                {
                    CrouchAction(!_crouch);
                }
                break;
        }
    }

    public virtual void CrouchAction(bool crouch)
    {
        if (_player.movementScript.onLader || (_player.GetComponent<WallJump>() && _player.GetComponent<WallJump>().onWall)) return;
        _crouch = crouch;
        _animator.SetBool("Crouching", crouch);
        foreach (var component in _player.components)
        {
            if (component.GetType() == typeof(Run))
            {
                Run runComponent = component as Run;
                if (crouch)
                { runComponent.RunAction(false); }
                else if(runComponent.run != runComponent.lastRunInput)
                { runComponent.RunAction(runComponent.lastRunInput); return; }
                break;
            }
        }

        _player.movementScript.speed = crouch ? _crouchWalkSpeed : _player.movementScript.walkSpeed;
    }

    private void CrouchSpeedControll()
    {
        if(_crouch)
        {
            _player.movementScript.speed = _player.movementScript.isGrounded ? _player.movementScript.speed = _crouchWalkSpeed : _player.movementScript.walkSpeed;
        }
    }

    protected virtual void OnGetOnStairs()
    {
        CrouchAction(false);
    }
}
