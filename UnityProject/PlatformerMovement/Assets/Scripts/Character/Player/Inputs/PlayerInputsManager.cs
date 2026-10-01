using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsManager : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump jump;
    private Run run;
    private Dash dash;

    private Vector2 lastMoveInputs;

    private bool enabledRun = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        run = GetComponent<Run>();
        dash = GetComponent<Dash>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputs = context.ReadValue<Vector2>();
        movement.moveDir = inputs;

        if(inputs.magnitude > 0.1f)
            lastMoveInputs = inputs;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) return;
        jump.Jump(context.started);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        enabledRun = !enabledRun;
        run.run(enabledRun);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        dash.DashIfCanTo(lastMoveInputs);
    }
}
