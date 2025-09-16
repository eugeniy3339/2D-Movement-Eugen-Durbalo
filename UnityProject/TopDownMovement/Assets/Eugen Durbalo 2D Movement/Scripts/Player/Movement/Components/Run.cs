using UnityEngine;
using UnityEngine.InputSystem;

/*
 
This script is a module for player movement and wouldnt work without a player, player movement and player inputs manager scripts.
 
 */

public class Run : MonoBehaviour
{
    private Player _playerScript;

    private bool _run = false;

    private void Awake()
    {
        _playerScript = GetComponentInChildren<Player>();
    }

    public void RunAction()
    {
        _run = !_run;

        if (_playerScript.playerMovementType == PlayerMovementType.Platformer)
        {
            GetComponentInChildren<PlatformerMovement>().run = _run;
            if (_run)
                GetComponentInChildren<PlatformerMovement>().speed = GetComponentInChildren<PlatformerMovement>().runSpeed;
            else
                GetComponentInChildren<PlatformerMovement>().speed = GetComponentInChildren<PlatformerMovement>().walkSpeed;
        }
        else if (_playerScript.playerMovementType == PlayerMovementType.Topdown)
        {
            GetComponentInChildren<TopDownMovement>().run = _run;
            if (_run)
                GetComponentInChildren<TopDownMovement>().speed = GetComponentInChildren<TopDownMovement>().runSpeed;
            else
                GetComponentInChildren<TopDownMovement>().speed = GetComponentInChildren<TopDownMovement>().walkSpeed;
        }
    }
}
