using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickInterpVec3
{


    private float timeItTakes;

    private bool smooth;
    public Vector3 status
    {
        get; private set;
    }

    public Vector3 startValue
    {
        get; private set;
    }
    public Vector3 endValue
    {
        get; private set;
    }
    public bool isDone
    {
        get { return duration >= 1f; } private set { }
    }
    public float duration
    {
        get; private set;
    }

    public QuickInterpVec3(Vector3 _start, Vector3 _end, float _time, bool _smooth = false)
    {
        startValue = _start;
        endValue = _end;
        timeItTakes = _time;
        smooth = _smooth;
    }

    public void InterpUpdate()
    {
        float statusX = 0f, statusY = 0f, statusZ = 0f;

        if (smooth)
        {
            statusX = Mathf.SmoothStep(startValue.x, endValue.x, duration);
            statusY = Mathf.SmoothStep(startValue.y, endValue.y, duration);
            statusZ = Mathf.SmoothStep(startValue.z, endValue.z, duration);
        }
        else if (!smooth)
        {
            statusX = Mathf.Lerp(startValue.x, endValue.x, duration);
            statusY = Mathf.Lerp(startValue.y, endValue.y, duration);
            statusZ = Mathf.Lerp(startValue.z, endValue.z, duration);
        }

        Vector3 newVec = new Vector3(statusX, statusY, statusZ);
        status = newVec;

        if (duration < 1f)
        {
            duration += Time.deltaTime / timeItTakes;
        }
    }

}
