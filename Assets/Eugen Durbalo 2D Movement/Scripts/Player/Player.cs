using UnityEngine;

public enum PlayerMovementType
{
    None,
    Platformer,
    Topdown
}

public class Player : MonoBehaviour
{
    public static Player Instance;

    public PlayerMovementType playerMovementType;
    public Transform gfx;

    private void Awake()
    {
        Instance = this;

        if (playerMovementType == PlayerMovementType.None)
        {
            if (GetComponent<PlatformerMovement>()) playerMovementType = PlayerMovementType.Platformer;
        }
    }
}
