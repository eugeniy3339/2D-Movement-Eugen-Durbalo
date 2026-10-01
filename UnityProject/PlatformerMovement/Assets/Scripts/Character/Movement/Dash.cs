using System;
using System.Collections;
using UnityEngine;

public class Dash : MonoBehaviour
{
    private Rigidbody2D rigidbody;

    private MovementManager movementManager;

    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 1f;
    [SerializeField] private float dashCooldown = 3f;
    private float curDashCooldown;

    public event Action onDashStart;
    public event Action onDashEnd;

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

    public void DashIfCanTo(Vector2 dir)
    {
        if(CanDash())
        {
            dash(dir);
        }
    }

    private bool CanDash()
    {
        return movementManager.movementState != MovementManager.MovementState.Dashing && curDashCooldown <= 0f;
    }

    private void dash(Vector2 dir)
    {
        StartCoroutine(DashCoro(dir));
    }

    private IEnumerator DashCoro(Vector2 dir)
    {
        onDashStart?.Invoke();
        float curTime = 0f;

        while (curTime < dashTime)
        {
            curTime += Time.deltaTime;
            rigidbody.linearVelocity = dir.normalized * dashSpeed;
            yield return new WaitForSeconds(0f);
        }

        rigidbody.linearVelocityY = 0f;
        curDashCooldown = dashCooldown;
        onDashEnd?.Invoke();
    }
}
