using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickInterp
{
    public float status
    {
        get; private set; 
    }
    private float startValue;
    private float endValue;
    private float timeItTakes;
    private float duration = 0.0f;
    public bool isDone
    {
        get { return duration >= 1f; } private set { }
    }

    public QuickInterp(float _start, float _end, float _time)
    {
        startValue = _start;
        endValue = _end;
        timeItTakes = _time;
    }

    private void Update()
    {
        status = Mathf.SmoothStep(startValue, endValue, status);

        if (duration < 1f)
        {
            duration += Time.deltaTime / timeItTakes;
        }
    }

}
