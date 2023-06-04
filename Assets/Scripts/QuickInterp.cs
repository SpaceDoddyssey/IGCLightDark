using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickInterp
{
    private float timeItTakes;

    private bool smooth;

    public float status
    {
        get; private set; 
    }
    public float startValue
    { 
        get; private set; 
    }
    public float endValue
    { 
        get; private set; 
    }
    public bool isDone
    {
        get { return duration >= 1f; }
        private set { }
    }
    public float duration
    {
        get; private set;
    } = 0.0f;

    public QuickInterp(float _start, float _end, float _time, bool _smooth = false)
    {
        startValue = _start;
        endValue = _end;
        timeItTakes = _time;
        smooth = _smooth;
    }

    public void InterpUpdate()
    {
        if (smooth)
        {
            status = Mathf.SmoothStep(startValue, endValue, status);
        }
        else if (!smooth)
        {
            status = Mathf.Lerp(startValue, endValue, status);
        }

        if (duration < 1f)
        {
            duration += Time.deltaTime / timeItTakes;
        }
    }

}
