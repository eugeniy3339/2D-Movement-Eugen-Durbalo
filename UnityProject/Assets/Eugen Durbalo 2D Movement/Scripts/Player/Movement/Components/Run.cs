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
        _playerScript = GetComponent<Player>();
    }

    public void RunAction()
    {
        _run = !_run;

        if (_playerScript.playerMovementType == PlayerMovementType.Platformer)
        {
            GetComponent<PlatformerMovement>().run = _run;
            if (_run)
                GetComponent<PlatformerMovement>().speed = GetComponent<PlatformerMovement>().runSpeed;
            else
                GetComponent<PlatformerMovement>().speed = GetComponent<PlatformerMovement>().walkSpeed;
        }
        else if (_playerScript.playerMovementType == PlayerMovementType.Topdown)
        {
            GetComponent<TopDownMovement>().run = _run;
            if (_run)
                GetComponent<TopDownMovement>().speed = GetComponent<TopDownMovement>().runSpeed;
            else
                GetComponent<TopDownMovement>().speed = GetComponent<TopDownMovement>().walkSpeed;
        }
    }
}
