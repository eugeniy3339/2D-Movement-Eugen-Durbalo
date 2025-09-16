using UnityEngine;
using UnityEngine.InputSystem;

/*
 
This script is a module for player movement and wouldnt work without a player, player movement and player inputs manager scripts.
 
 */

public class Run : PlayerComponent
{
    private Player _player;

    [SerializeField] private float _runSpeed = 3.5f;
    private bool _run = false;

    private void Start()
    {
        _player = Player.Instance;
    }

    public void RunAction()
    {
        _run = !_run;

        _player.movementScript.run = _run;
        _player.movementScript.speed = _run ? _runSpeed : _player.movementScript.walkSpeed;
    }
}
