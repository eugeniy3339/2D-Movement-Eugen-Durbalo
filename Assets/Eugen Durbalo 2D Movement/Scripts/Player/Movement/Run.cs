using UnityEngine;
using UnityEngine.InputSystem;

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
    }
}
