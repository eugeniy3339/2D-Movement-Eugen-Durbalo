using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

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
    public bool run = false;
    public bool lastRunInput = false;

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
                lastRunInput = !context.canceled;
                break;
            case RunMode.Toggle:
                if (context.started) 
                { 
                    lastRunInput = !lastRunInput; 
                    RunAction(!run); 
                }
                break;
        }
    }

    public void RunAction(bool run)
    {
        this.run = run;
        _player.movementScript.run = this.run;
        foreach (var component in _player.components)
        {
            if (component.GetType() == typeof(Crouch))
            {
                Crouch crouchComponent = component as Crouch;
                if (run)
                    crouchComponent.CrouchAction(false);
            }
        }

        _player.movementScript.speed = this.run ? _runSpeed : _player.movementScript.walkSpeed;
    }
}
