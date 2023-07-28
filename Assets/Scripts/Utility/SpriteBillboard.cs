using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    public bool miniMap = false;
    // Update is called once per frame
    void Update()
    {
        if (miniMap)
        {
            if (Camera.current != null)
            {
                transform.rotation = Quaternion.Euler(90f, Camera.current.transform.rotation.eulerAngles.y, 0f);
            }
        }
        else
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);

    }
}
