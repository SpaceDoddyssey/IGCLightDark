using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

public class PolarityBarTicker : MonoBehaviour
{
    private bool isLerping = false;
    private float lerpAmt;
    private GameState stateObject;

    // This is a modifier determining how long it takes to interpolate between ticks.
    // Lower values make faster lerps.
    public float lerpSpeed;
    // An int value between -4 and 4.
    private int targetPolarity, prevPolarity;
    private GameObject gradient;

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

    private Dictionary<int, float> gradientPositions = new Dictionary<int, float>()
    {
        { -4, 31.5f },
        { -3, 24f },
        { -2, 19f },
        { -1, 10f },
        {  0,  0f },
        {  1,  -10f },
        {  2,  -19f },
        {  3,  -24f },
        {  4,  -31f }
    };

    private Color mostDark = new Color(60f, 59f, 60f, 1f);

    private Dictionary<int, Color> fogColors = new Dictionary<int, Color>()
    {
        { -4, new Color(60f, 59f, 60f, 1f) },
        { -3, new Color(82f, 81f, 82f, 1f) },
        { -2, new Color(96f, 97f, 96f, 1f) },
        { -1, new Color(126f, 125f, 126f, 1f) },
        { 0, new Color(128, 128, 128, 1f) },
        { 1, new Color(126f, 125f, 126f, 1f) },
        { 2, new Color(176f, 175f, 176f, 1f) },
        { 3, new Color(195f, 196f, 195f, 1f) },
        { 4, new Color(218f, 219f, 218f, 1f) }

    };


    // Start is called before the first frame update
    void Start()
    {
        stateObject = GameObject.Find("Game World Manager").GetComponent<GameState>();
        gradient = GameObject.Find("BG");
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
            gradient.transform.localPosition = new Vector3(0, Mathf.SmoothStep(gradientPositions[prevPolarity], gradient.transform.localPosition.y + gradientPositions[targetPolarity], lerpAmt), gradient.transform.localPosition.z);
            

            // If the lerp is done, reset everything.
            if (lerpAmt > 1f)
            {
                isLerping = false;
            }

        }
        else
        {
            stateObject.polarity = (int)Mathf.Clamp((float)stateObject.polarity, -4f, 4f);
            transform.localPosition = new Vector3(screenTickPositions[stateObject.polarity], 0, 0);
            gradient.transform.localPosition = new Vector3(gradient.transform.localPosition.x, gradient.transform.localPosition.y + gradientPositions[stateObject.polarity], gradient.transform.localPosition.z);
            //RenderSettings.fogColor = mostDark;
            //print(fogColors[stateObject.polarity]);

        }
    }

    public void SetTickerOffset(int delta, bool interpolate = true)
    {
        if (delta == 0 || isLerping) return;

        if (stateObject.polarity + delta < -4)
        {
            targetPolarity = -4;
        }
        else if (stateObject.polarity + delta > 4)
        {
            targetPolarity = 4;
        }
        else
        {
            targetPolarity = stateObject.polarity + delta;
            prevPolarity = stateObject.polarity;
            stateObject.polarity = targetPolarity;
            Debug.Log("Target:" + targetPolarity + "  prev:" + prevPolarity + "  pol:"+ stateObject.polarity);
        }
        
        if (interpolate == true)
        {
            BeginLerp();
        }
        else
        {
            // Otherwise, instantly set the polarity and position
            stateObject.polarity = targetPolarity;
            transform.localPosition = new Vector3(screenTickPositions[stateObject.polarity], transform.position.y, transform.position.z);
            gradient.transform.localPosition = new Vector3(transform.localPosition.x, gradient.transform.localPosition.y + gradientPositions[stateObject.polarity], transform.localPosition.z);
            //RenderSettings.fogColor = Color.black;

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
            stateObject.polarity = -4;
        }
        else if (stateObject.polarity > 4)
        {
            stateObject.polarity = 4;
        }
        else
        {
            stateObject.polarity = value;
        }

        // Manual tick settings move the ticker immediately for now. 
        transform.position = new Vector3(screenTickPositions[stateObject.polarity], transform.position.y, transform.position.z);
        gradient.transform.localPosition = new Vector3(transform.position.x, gradient.transform.localPosition.y + gradientPositions[stateObject.polarity], transform.position.z);
        //RenderSettings.fogColor = fogColors[stateObject.polarity];

        // Kill any lerping that's happening.
        isLerping = false;
        targetPolarity = stateObject.polarity;
        prevPolarity = stateObject.polarity;

        Debug.Log("Polarity = " + stateObject.polarity);
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
