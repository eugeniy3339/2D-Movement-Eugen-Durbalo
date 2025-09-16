using System.Collections.Generic;
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

[RequireComponent(typeof(Movement))]
public class Player : MonoBehaviour
{
    public static Player Instance;
    public Movement movementScript;
    public List<PlayerComponent> components;

    [Tooltip("Player movement type (You dont have to select one if you have a movement script on player gameObject)")] public PlayerMovementType playerMovementType;
    [Tooltip("Your character Graphics (need to rotate your character while dashing)")] public Transform gfx;

    private void Awake()
    {
        Instance = this;
        movementScript = GetComponent<Movement>();

        components.Clear();
        foreach (var component in GetComponents<PlayerComponent>())
        {
            components.Add(component);
        }
    }
}
