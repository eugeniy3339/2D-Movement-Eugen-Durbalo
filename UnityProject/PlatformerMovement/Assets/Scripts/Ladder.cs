using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Collider2D))]
public class Ladder : MonoBehaviour
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
        if(playerMovement && !playerMovement.curLadders.Contains(this))
        {
            playerMovement.AddLadder(this);
            if (playerMovement.getOnTheLadderMehode != GetOnTheLadderMethode.Input)
                playerMovement.GetOnLadder();
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
