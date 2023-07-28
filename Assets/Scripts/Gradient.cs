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

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        stateObject = GameObject.Find("Game World Manager").GetComponent<GameState>();

        bgRenderer = transform.GetComponent<SpriteRenderer>();

        float camHeight = camera.orthographicSize;
        float camWidth = camera.aspect * camera.orthographicSize;
        while (bgRenderer.sprite.bounds.size.x * multipleX < camWidth * 2)
            multipleX++;
        //while (bgRenderer.sprite.bounds.size.y * multipleY < camHeight * 2)
            //multipleY++;
        bgRenderer.size = new Vector2(bgRenderer.sprite.bounds.size.x * multipleX, bgRenderer.sprite.bounds.size.y * multipleY);


    }

    // Update is called once per frame
    void LateUpdate()
    {
        // transform.localPosition = new Vector3(
        //transform.localPosition.x + Time.timeSinceLevelLoad * stateObject.GetGradientSpeed() * Time.deltaTime, 
        // transform.localPosition.y + (Time.timeSinceLevelLoad) * Time.deltaTime, 
        //transform.localPosition.z);

        if (stateObject.polarity == 0) return;

        //bgRenderer.size += new Vector2(1f * Time.deltaTime * stateObject.GetGradientSpeed() * (Mathf.Sign(stateObject.polarity)), 0.0f);
        //Vector2 newVec = bgRenderer.size;

        //if (Mathf.Abs(bgRenderer.size.x) > Mathf.Abs(bgRenderer.sprite.bounds.size.x * multipleX))
        //{
        //    newVec.x = bgRenderer.sprite.bounds.size.x * multipleX * 2;
        //}

        //if (Mathf.Abs(bgRenderer.size.x) < Mathf.Abs(bgRenderer.sprite.bounds.size.x))
        //{
        //    newVec.x = bgRenderer.sprite.bounds.size.x * multipleX;
        //}

        //if (bgRenderer.size.y > bgRenderer.sprite.bounds.size.y * multipleY * 2)
        //{
        //    newVec.y = bgRenderer.sprite.bounds.size.y * multipleY;
        //}

        //bgRenderer.size = newVec;

    }
}
