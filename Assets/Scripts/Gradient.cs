using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gradient : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x + (Mathf.Sin(Time.timeSinceLevelLoad) * Time.deltaTime), 
            transform.localPosition.y + (Mathf.Sin(Time.timeSinceLevelLoad) * Time.deltaTime), 
            transform.localPosition.z);
    }
}
