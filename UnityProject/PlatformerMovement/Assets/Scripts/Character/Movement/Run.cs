using System;
using UnityEngine;

public class Run : MonoBehaviour
{
    [SerializeField] private float _runningSpeed = 10f;
    public float runningSpeed { get { return _runningSpeed; } }

    private bool running = false;

    public event Action onStartedRunning;
    public event Action onStoppedRunning;

    public void run()
    {
        run(!running);
    }

    public void run(bool start)
    {
        running = start;
        if(start)
            onStartedRunning?.Invoke();
        else
            onStoppedRunning?.Invoke();
    }


}
