using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsManager : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump jump;
    private Run run;

    private bool enabledRun = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        run = GetComponent<Run>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement.moveDir = context.ReadValue<Vector2>();
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
}
