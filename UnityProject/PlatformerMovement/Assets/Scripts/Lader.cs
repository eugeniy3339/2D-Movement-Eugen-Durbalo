using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Lader : MonoBehaviour
{
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        if(!_collider.isTrigger)
            _collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Movement playerMovement = collision.GetComponent<Movement>();
        if(playerMovement && !playerMovement.curLaders.Contains(this))
        {
            playerMovement.AddLader(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Movement playerMovement = collision.GetComponent<Movement>();
        if (playerMovement && playerMovement.curLaders.Contains(this))
        {
            playerMovement.RemoveLader(this);
        }
    }
}
