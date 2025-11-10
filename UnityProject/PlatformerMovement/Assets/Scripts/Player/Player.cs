using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
 
This script is needed to set up your character. It is used as a player Singleton (if you dont knowq what is Singletons check out this video: https://www.youtube.com/watch?v=mpM0C6quQjs);

 */

[RequireComponent(typeof(Movement))]
public class Player : MonoBehaviour
{
    public static Player Instance;
    public Movement movementScript;
    public List<PlayerComponent> components = new List<PlayerComponent>();

    [Tooltip("Your character Graphics (need to rotate your character while dashing)")] public Transform gfx;
    private SpriteRenderer _gfxSpriteRenderer;

    [HideInInspector] public bool canFlipGfx = true;

    private bool _gfxFlipX;
    public bool gfxFlipX {
        get {
            return _gfxFlipX;
        }
        set {
            if(canFlipGfx)
            {
                _gfxFlipX = value;
                _gfxSpriteRenderer.flipX = value;
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        movementScript = GetComponent<Movement>();
        _gfxSpriteRenderer = gfx.GetComponent<SpriteRenderer>();

        canFlipGfx = true;

        foreach (var component in GetComponentsInChildren<PlayerComponent>())
        {
            if(!components.Contains(component))
                components.Add(component);
        }
    }
}
