using UnityEngine;

public class PlayerJump : Jump
{
    [SerializeField] private float kayoteTime = 0.1f;
    private float curKayoteTime;

    [SerializeField] private float inputFuncTime = 0.1f;
    private float curInputFuncTime;

    private bool holdingJumpButton;

    protected override void Update()
    {
        base.Update();

        if(curKayoteTime > 0f)
            curKayoteTime -= Time.deltaTime;

        if(curInputFuncTime > 0f)
            curInputFuncTime -= Time.deltaTime;
    }

    protected override bool CanJump()
    {
        return (movementManager.isGrounded || (curKayoteTime > 0f && !fallingAfterTheJump)) && movementManager.movementState == MovementManager.MovementState.Default && !jumping && curMinJumpTime <= 0f && curJumpCooldown <= 0f;
    }

    protected override void OnMinJumpTime()
    {
        base.OnMinJumpTime();
        if (jumping && !holdingJumpButton) 
            StopJump();
    }

    public void Jump(bool start)
    {
        holdingJumpButton = start;
        if (start)
        {
            if(!JumpIfCanTo())
                curInputFuncTime = inputFuncTime;
        }
        else
            StopJump();
    }

    protected override void OnGrounded()
    {
        base.OnGrounded();
        if (curInputFuncTime > 0f)
            JumpIfCanTo();
    }

    private void OnUngrounded()
    {
        curKayoteTime = kayoteTime;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        movementManager.onUngrounded += OnUngrounded;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        movementManager.onUngrounded -= OnUngrounded;
    }
}
