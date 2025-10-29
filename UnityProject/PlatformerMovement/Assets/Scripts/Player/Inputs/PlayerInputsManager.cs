using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsManager : MonoBehaviour
{
    private Movement _movement;
    private Jump _jump;
    private Run _run;
    private Crouch _crouch;
    private Dash _dash;

    private void Awake()
    {
        _movement = GetComponent<Movement>();
        _jump = GetComponent<Jump>();
        _run = GetComponent<Run>();
        _crouch = GetComponent<Crouch>();
        _dash = GetComponent<Dash>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!_movement) return;

        _movement.OnMove(context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!_jump) return;

        _jump.OnJump(context);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!_crouch) return;

        _crouch.OnCrouch(context);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (!_run) return;

        _run.OnRun(context);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!_dash) return;

        _dash.OnDash(context);
    }
}
