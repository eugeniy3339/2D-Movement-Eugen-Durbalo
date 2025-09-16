using UnityEngine;
using UnityEngine.InputSystem;

/*
 
This script is a module for player movement and wouldnt work without a player, player movement and player inputs manager scripts.
 
 */

public enum RunMode
{
    Toggle,
    Hold
}

[RequireComponent(typeof(Movement))]
public class Run : PlayerComponent
{
    private Player _player;

    [SerializeField] private RunMode _runMode;
    [SerializeField] private float _runSpeed = 3.5f;
    private bool _run = false;

    private void Start()
    {
        _player = Player.Instance;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        switch (_runMode)
        {
            case RunMode.Hold:
                RunAction(!context.canceled);
                break;
            case RunMode.Toggle:
                if (context.started) { RunAction(!_run); }
                break;
        }
    }

    private void RunAction(bool run)
    {
        _run = run;

        _player.movementScript.run = _run;
        _player.movementScript.speed = _run ? _runSpeed : _player.movementScript.walkSpeed;
    }
}
