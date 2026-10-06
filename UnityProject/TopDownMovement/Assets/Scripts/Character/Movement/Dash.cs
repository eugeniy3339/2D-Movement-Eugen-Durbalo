using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementManager))]
public class Dash : MonoBehaviour
{
    private Rigidbody2D rigidbody;

    private MovementManager movementManager;

    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.3f;
    [SerializeField] private float dashCooldown = 3f;
    private float curDashCooldown;

    public event Action<Vector2> onDashStarted;
    public event Action onDashEnded;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        movementManager = GetComponent<MovementManager>();
    }

    private void Update()
    {
        if (curDashCooldown > 0f)
            curDashCooldown -= Time.deltaTime;
    }

    public void DashIfCanTo(Vector2 direction)
    {
        if (CanDash())
            dash(direction);
    }

    private bool CanDash()
    {
        return curDashCooldown <= 0f && movementManager.movementState == MovementManager.MovementState.Default;
    }

    private void dash(Vector2 direction)
    {
        StartCoroutine(DashCoro(direction));
    }

    private IEnumerator DashCoro(Vector2 direction)
    {
        onDashStarted?.Invoke(direction);
        float curTime = 0f;

        while(curTime < dashTime)
        {
            curTime += Time.deltaTime;
            rigidbody.linearVelocity = direction.normalized * dashSpeed;
            yield return new WaitForEndOfFrame();
        }

        rigidbody.linearVelocity = Vector2.zero;
        curDashCooldown = dashCooldown;
        onDashEnded?.Invoke();
    }
}
