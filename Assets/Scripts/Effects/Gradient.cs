using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gradient : MonoBehaviour
{
    private GameState stateObject;
    private SpriteRenderer bgRenderer;
    private new Camera camera;
    private int multipleX = 1;
    private int multipleY = 1;
    private float camWidth;

    public float waveFrequency;
    public float waveAmplitude;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        stateObject = GameObject.Find("Game World Manager").GetComponent<GameState>();

        bgRenderer = transform.GetComponent<SpriteRenderer>();

        camWidth = camera.aspect * camera.orthographicSize;
        while (bgRenderer.sprite.bounds.size.x * multipleX < camWidth * 2)
            multipleX++;

        bgRenderer.size = new Vector2(bgRenderer.sprite.bounds.size.x * multipleX, bgRenderer.sprite.bounds.size.y * multipleY);


    }

    // Update is called once per frame
    void LateUpdate()
    {

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + (Mathf.Sin(Time.timeSinceLevelLoad * waveFrequency) * waveAmplitude), transform.localPosition.z);

        if (stateObject.polarity == 0)
        {
            return;
        }




        Vector2 offset = new Vector2(1.5f * Time.deltaTime * stateObject.GetGradientSpeed() * (Mathf.Sign(stateObject.polarity)), 0.0f);
        bgRenderer.size += offset;
        Vector2 newVec = bgRenderer.size;

        if (offset.x > 0f)
        {
            if (bgRenderer.size.x > bgRenderer.sprite.bounds.size.x * multipleX * 2)
            {
                newVec.x = bgRenderer.sprite.bounds.size.x * multipleX;
            }
        }
        else if (offset.x < 0f)
        {
            if (bgRenderer.size.x < bgRenderer.sprite.bounds.size.x * multipleX * 0.75)
            {
                newVec.x = bgRenderer.sprite.bounds.size.x * multipleX;
            }
        }

        

        bgRenderer.size = newVec;

    }
}
