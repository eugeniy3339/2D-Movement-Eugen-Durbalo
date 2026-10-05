using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsManager : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump jump;
    private Run run;
    private Dash dash;

    private PlayerInputs playerInputs;

    private Vector2 lastMoveInputs;
    private bool enabledRun = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        run = GetComponent<Run>();
        dash = GetComponent<Dash>();



        playerInputs = new PlayerInputs();

        playerInputs.Default.Movement.started += OnMove;
        playerInputs.Default.Movement.performed += OnMove;
        playerInputs.Default.Movement.canceled += OnMove;

        playerInputs.Default.Jump.started += OnJump;
        playerInputs.Default.Jump.canceled += OnJump;

        playerInputs.Default.Run.started += OnRun;

        playerInputs.Default.Dash.started += OnDash;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputs = context.ReadValue<Vector2>();
        if (movement != null)
            movement.moveDir = inputs;

        if(inputs.magnitude > 0.1f)
            lastMoveInputs = inputs;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (jump == null) return;
        if (context.performed) return;

        jump.Jump(context.started);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (run == null) return;
        if (!context.started) return;

        enabledRun = !enabledRun;
        run.run(enabledRun);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (dash == null) return;
        if (!context.started) return;

        dash.DashIfCanTo(lastMoveInputs);
    }



    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

    private void OnDestroy()
    {
        playerInputs.Default.Movement.started -= OnMove;
        playerInputs.Default.Movement.performed -= OnMove;
        playerInputs.Default.Movement.canceled -= OnMove;

        playerInputs.Default.Jump.started -= OnJump;
        playerInputs.Default.Jump.canceled -= OnJump;

        playerInputs.Default.Run.started -= OnRun;

        playerInputs.Default.Dash.started -= OnDash;
    }
}
