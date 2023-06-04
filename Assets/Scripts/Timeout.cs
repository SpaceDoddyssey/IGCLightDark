using UnityEngine;

public class Timeout
{
    [Tooltip("The time, in seconds, that the timeout lasts.")]
    public float time;
    private float _timeoutDelta;

    // Start is called before the first frame update
    public Timeout(float _time = 1)
    {
        time = _time;
    }

    public void ResetTimeout()
    {
        _timeoutDelta = time;
    }

    //Ticks down the timer by one millisecond.
    public void Tick()
    {
        if (_timeoutDelta > 0)
            _timeoutDelta -= Time.deltaTime;
        else if (_timeoutDelta <= 0)
        {
            _timeoutDelta = 0;
        }

    }

    //If timer > 0, does nothing. if timer <= zero, returns true 
    public bool CheckIsZeroReset()
    {
        if (_timeoutDelta <= 0)
        {
            ResetTimeout();
            return true;
        }
        else return false;
    }

    public bool CheckIsZero()
    {
        return _timeoutDelta <= 0? true: false;
    }

    public float CurrentTime()
    {
        return _timeoutDelta;
    }

}
