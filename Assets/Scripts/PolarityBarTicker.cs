using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarityBarTicker : MonoBehaviour
{
    private bool isLerping = false;
    private float lerpAmt;

    // This is a modifier determining how long it takes to interpolate between ticks.
    // Lower values make faster lerps.
    public float lerpSpeed;
    // An int value between -4 and 4.
    public int polarity
    {
        get; private set;
    }
    private int targetPolarity, prevPolarity;

    // Right now the values for where the ticker should be are hard-coded.
    // In the future, I'm going to change these to be scaled values of the 
    // width of the bar itself.
    private Dictionary<int, float> screenTickPositions = new Dictionary<int, float>()
    {
        { -4, -256f },
        { -3, -182f },
        { -2, -118f },
        { -1, -68f },
        {  0,  0f },
        {  1,  68f },
        {  2,  118f },
        {  3,  182f },
        {  4,  256f }
    };


    // Start is called before the first frame update
    void Start()
    {
        polarity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLerping)
        {
            // If currently in a lerp, add to the lerp and divide it by how quickly the lerp should happen.
            lerpAmt += Time.deltaTime / lerpSpeed;
            // Change the transform's local position based on the dict of screen tick positions and the lerp amount.
            transform.localPosition = new Vector3(Mathf.Lerp(screenTickPositions[prevPolarity], screenTickPositions[targetPolarity], lerpAmt), 0, 0);
            // If the lerp is done, reset everything.
            if (lerpAmt > 1f)
            {
                isLerping = false;
            }

        }
    }

    public void SetTickerOffset(int delta, bool interpolate = true)
    {
        if (delta == 0 || isLerping) return;

        if (polarity + delta < -4)
        {
            targetPolarity = -4;
        }
        else if (polarity + delta > 4)
        {
            targetPolarity = 4;
        }
        else
        {
            targetPolarity =  polarity + delta;
            prevPolarity = polarity;
            polarity = targetPolarity;
            Debug.Log("Target:" + targetPolarity + "  prev:" + prevPolarity + "  pol:"+polarity);
        }
        
        if (interpolate == true)
        {
            BeginLerp();
        }
        else
        {
            // Otherwise, instantly set the polarity and position
            polarity = targetPolarity;
            transform.localPosition = new Vector3(screenTickPositions[polarity], transform.position.y, transform.position.z);
            // Kill any lerping that's happening.
            isLerping = false;
        }


    }

    private void BeginLerp()
    {
        lerpAmt = 0f;
        isLerping = true;
    }

    // Useful little function.
    public void SetTickerManual(int value)
    {
        if (value < -4)
        {
            polarity = -4;
        }
        else if (polarity > 4)
        {
            polarity = 4;
        }
        else
        {
            polarity = value;
        }

        // Manual tick settings move the ticker immediately for now. 
        transform.position = new Vector3(screenTickPositions[polarity], transform.position.y, transform.position.z);
        // Kill any lerping that's happening.
        isLerping = false;
        targetPolarity = polarity;
        prevPolarity = polarity;

        Debug.Log("Polarity = " + polarity);
    }

    //Possibly useless?
    public void ShiftTickerByOne(bool shiftingRight)
    {
        if (!isLerping)
        {
            if (shiftingRight == true)
            {
                SetTickerOffset(1);
            }
            else if (shiftingRight == false)
            {
                SetTickerOffset(-1);
            }

        }
    }


}
