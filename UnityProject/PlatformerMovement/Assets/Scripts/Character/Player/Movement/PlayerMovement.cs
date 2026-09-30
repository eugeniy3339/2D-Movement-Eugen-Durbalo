using UnityEngine;

public class PlayerMovement : Movement
{
    [HideInInspector] public Vector2 moveDir;

    protected override void Move()
    {
        Move(moveDir.x);
    }
}
