using System;
using UnityEngine;

public class Run : MonoBehaviour
{

    [SerializeField] private float _runSpeed = 10f;
    public float runSpeed { get { return _runSpeed; } }

    private bool running;

    public event Action onStartedRunning;
    public event Action onStoppedRunning;

    public void run()
    {
        run(!running);
    }

    public void run(bool run)
    {
        running = run;
        if (run)
            onStartedRunning?.Invoke();
        else
            onStoppedRunning?.Invoke();
    }
}
