using UnityEngine;

public class Stairs : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision);
        AddStairs(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        RemoveStairs(collision);
    }

    private void AddStairs(Collider2D collision)
    {
        Movement playerMovement = collision.GetComponent<Movement>();

        if (playerMovement)
        {
            playerMovement.curStairsList.Add(this);
        }
    }

    private void RemoveStairs(Collider2D collision)
    {
        Movement playerMovement = collision.GetComponent<Movement>();

        if (playerMovement && playerMovement.curStairsList.Contains(this))
        {
            playerMovement.curStairsList.Remove(this);
        }
    }
}
