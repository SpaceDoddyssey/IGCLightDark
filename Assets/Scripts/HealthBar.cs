using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private RectTransform myTransform;
    private RectTransform childTransform;
    private GameState stateObject;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<RectTransform>();
        childTransform = transform.GetChild(0).GetComponent<RectTransform>();
        stateObject = GameObject.Find("Game World Manager").GetComponent<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
        float newX = ((float)stateObject.playerHealth / (float)stateObject.playerMaxAbsoluteHealth) * (myTransform.rect.width / 2f);
        childTransform.localPosition = new Vector3(newX, 0, 0);


    }
}
