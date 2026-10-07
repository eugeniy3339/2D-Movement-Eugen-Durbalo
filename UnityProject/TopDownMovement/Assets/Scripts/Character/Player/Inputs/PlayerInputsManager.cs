using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsManager : MonoBehaviour
{
    private PlayerMovement movement;
    private Dash dash;
    private Run run;

    private PlayerInputs playerInputs;

    private Vector2 lastMoveInputs;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        dash = GetComponent<Dash>();
        run = GetComponent<Run>();

        playerInputs = new PlayerInputs();

        playerInputs.Default.Movement.started += OnMove;
        playerInputs.Default.Movement.performed += OnMove;
        playerInputs.Default.Movement.canceled += OnMove;

        playerInputs.Default.Dash.started += OnDash;

        playerInputs.Default.Run.started += OnRun;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (movement != null)
            movement.moveDir = input;

        if (input.magnitude > 0.1f)
            lastMoveInputs = input;
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (dash == null) return;
        if (!context.started) return;

        dash.DashIfCanTo(lastMoveInputs);
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        if (run == null) return;
        if (!context.started) return;

        run.run();
    }

    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }
}
