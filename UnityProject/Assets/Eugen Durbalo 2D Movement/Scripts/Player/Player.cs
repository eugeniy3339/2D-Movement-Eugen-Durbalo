using Unity.VisualScripting;
using UnityEngine;

/*
 
This script is needed to set up your character. It is used as a player Singleton (if you dont knowq what is Singletons check out this video: https://www.youtube.com/watch?v=mpM0C6quQjs);

 */

public enum PlayerMovementType
{
    None,
    Platformer,
    Topdown
}

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Tooltip("Player movement type (You dont have to select one if you have a movement script on player gameObject)")] public PlayerMovementType playerMovementType;
    [Tooltip("Your character Graphics (need to rotate your character while dashing)")] public Transform gfx;

    private void Awake()
    {
        Instance = this;

        if(playerMovementType != PlayerMovementType.None)
        {
            if (playerMovementType == PlayerMovementType.Platformer && !GetComponent<PlatformerMovement>()) transform.AddComponent<PlatformerMovement>();
            else if (playerMovementType == PlayerMovementType.Topdown && !GetComponent<TopDownMovement>()) transform.AddComponent<TopDownMovement>();
        }
        else
        {
            if (GetComponent<PlatformerMovement>()) playerMovementType = PlayerMovementType.Platformer;
            else if (GetComponent<TopDownMovement>()) playerMovementType = PlayerMovementType.Topdown;
            else print("You need to add Movement Script to Player GameObject or set Player Movement Type from None (can be found in Player GameObject).");
        }
    }
}
