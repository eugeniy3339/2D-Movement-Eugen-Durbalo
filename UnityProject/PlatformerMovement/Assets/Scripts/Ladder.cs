using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Ladder : MonoBehaviour
{
    private Collider2D _collider;
    public static Action<Ladder> onAddedLadder;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        if(!_collider.isTrigger)
            _collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Movement playerMovement = collision.GetComponent<Movement>();
        if(playerMovement && !playerMovement.curLadders.Contains(this))
        {
            playerMovement.AddLadder(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Movement playerMovement = collision.GetComponent<Movement>();
        if (playerMovement && playerMovement.curLadders.Contains(this))
        {
            playerMovement.RemoveLadder(this);
        }
    }
}
