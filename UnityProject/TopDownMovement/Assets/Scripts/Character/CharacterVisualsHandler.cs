using UnityEngine;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(Animator))]
public class CharacterVisualsHandler : MonoBehaviour
{
    private Animator animator;

    private Movement movement;
    private Dash dash;

    private Vector2 moveDir;

    private bool _d;
    private bool dashing
    {
        get { return _d; }
        set
        {
            _d = value;
            animator.SetBool("IsDashing", value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        movement = GetComponent<Movement>();
        dash = GetComponent<Dash>();
    }



    private void OnMoveDirChanged(Vector2 direction)
    {
        moveDir = direction;
        if(!dashing)
            SetLookDir(direction);
    }

    private void SetLookDir(Vector2 direction)
    {
        animator.SetFloat("velocity", direction.magnitude);
        if (direction.magnitude <= 0.1f) return;
        animator.SetFloat("x", direction.x);
        animator.SetFloat("y", direction.y);
    }

    private void OnDashStarted(Vector2 direction)
    {
        dashing = true;
        SetLookDir(direction);

        animator.Play("StartDash");
    }

    private void OnDashEnded()
    {
        dashing = false;

        SetLookDir(moveDir);
    }



    private void OnEnable()
    {
        if(movement != null)
        {
            movement.onMoveDirChanged += OnMoveDirChanged;
        }
        if(dash != null)
        {
            dash.onDashStarted += OnDashStarted;
            dash.onDashEnded += OnDashEnded;
        }
    }

    private void OnDisable()
    {
        if (movement != null)
        {
            movement.onMoveDirChanged -= OnMoveDirChanged;
        }
        if (dash != null)
        {
            dash.onDashStarted -= OnDashStarted;
            dash.onDashEnded -= OnDashEnded;
        }
    }
}
